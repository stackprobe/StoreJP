/*
	display Dev Batch file Hash

	相違を発見した場合、最初に発見した相違のあるファイルセットを FOUNDLISTFILE へ書き出す。
	相違無しの場合 FOUNDLISTFILE は削除される。
*/

#include "C:\Factory\Common\all.h"

static autoList_t *CleanBatches;
static autoList_t *ReleaseBatches;
static autoList_t *ReleaseBatchGroupNames;
static int ExistDiffOverall;

static char *GetFileMD5(char *file)
{
	char *rdrFile = makeTempPath("tmp");
	char *hash;

	execute_x(xcout("C:\\Factory\\Tools\\md5.exe \"%s\" > \"%s\"", file, rdrFile));

	hash = readFirstLine(rdrFile);

	errorCase(!lineExp("<32,09af>", hash));

	removeFile(rdrFile);
	memFree(rdrFile);

	return hash;
}
static void DiffFileSetToFoundFileList(autoList_t *batches)
{
	writeLines(FOUNDLISTFILE, batches);
}
static void ShowBatchFiles(char *name, autoList_t *batches)
{
	char *batch;
	uint index;
	char *firstHash = NULL;
	int existDiff = 0;

	cout("\n");
	cout("----\n");
	cout("%s\n", name);

	foreach (batches, batch, index)
	{
		char *hash = GetFileMD5(batch);

		cout("%s %s\n", hash, batch);

		if (firstHash)
			existDiff |= _stricmp(firstHash, hash);
		else
			firstHash = strx(hash);

		memFree(hash);
	}
	memFree(firstHash);

	if (existDiff)
	{
		cout("+----------+\n");
		cout("| 相違あり |\n");
		cout("+----------+\n");

		if (!ExistDiffOverall)
		{
			DiffFileSetToFoundFileList(batches);
			ExistDiffOverall = 1;
		}
	}
}
static void ShowAllBatchFiles(void)
{
	ShowBatchFiles("Clean.bat", CleanBatches);

	// ReleaseBatchGroupNames の種類毎に ReleaseBatches を処理する。
	{
		autoList_t *groupNames = newList();
		char *groupName;
		uint groupName_index;

		{
			autoList_t *tmpList = copyAutoList(ReleaseBatchGroupNames);
			rapidSortLines(tmpList);
			distinctLines(tmpList, groupNames, NULL);
			releaseAutoList(tmpList);
		}

		foreach (groupNames, groupName, groupName_index)
		{
			char *name = xcout("Release.bat (%s)", groupName);
			autoList_t *batches = newList();
			char *batch;
			uint batch_index;

			foreach (ReleaseBatches, batch, batch_index)
				if (!strcmp(groupName, getLine(ReleaseBatchGroupNames, batch_index)))
					addElement(batches, (uint)batch);

			ShowBatchFiles(name, batches);

			memFree(name);
			releaseAutoList(batches);
		}
		releaseAutoList(groupNames);
	}

	if (ExistDiffOverall)
	{
		cout("\n");
		cout("#####################\n");
		cout("## 1件以上相違あり ##\n");
		cout("#####################\n");
	}
}
int main(int argc, char **argv)
{
	autoList_t *dirs = slssDirs("C:\\Dev");
	char *dir;
	uint index;

	removeFileIfExist(FOUNDLISTFILE); // あらかじめ削除しておく。

	foreach (dirs, dir, index)
	{
		if (!*dir) // ? 除外済み
		{
			// noop
		}
		else if (lineExp("<1,,AZaz>20200001", getLocal(dir))) // ? プロジェクトDIR
		{
			char *dirYen = xcout("%s\\", dir);
			char *dir2;
			uint index2;

			foreach (dirs, dir2, index2)
				if (startsWithICase(dir2, dirYen))
					*dir2 = '\0'; // 配下を除外

			memFree(dirYen);
		}
		else
		{
			*dir = '\0'; // 除外
		}
	}
	trimLines(dirs);

	CleanBatches           = newList();
	ReleaseBatches         = newList();
	ReleaseBatchGroupNames = newList();

	foreach (dirs, dir, index)
	{
		char *cleanFile   = changeLocal(dir, "Clean.bat");
		char *releaseFile = changeLocal(dir, "Release.bat");

		errorCase(!existFile(cleanFile));
		errorCase(!existFile(releaseFile));

		addElement(CleanBatches, (uint)cleanFile);
		addElement(ReleaseBatches, (uint)releaseFile);
		addElement(ReleaseBatchGroupNames, (uint)strx(getLocal(dir)));
	}
	releaseDim(dirs, 1);

	ShowAllBatchFiles();
}
