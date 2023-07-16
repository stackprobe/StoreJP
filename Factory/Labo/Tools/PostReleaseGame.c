#include "C:\Factory\Common\all.h"

static void InsertLinesToLines(autoList_t *destLines, uint insertPos, autoList_t *srcLines)
{
	uint index;

	for (index = getCount(srcLines); index; index--)
	{
		insertElement(destLines, insertPos, (uint)strx(getLine(srcLines, index - 1)));
	}
}

static char *OutDir;
static char *RootDir; // このプロジェクトのルートDIR
static char *ResourceDir;
static char *ManualFile; // マニュアル (Readme.txt)

// ---- SetSourceOfResource ----

#define SSOR_REPLACE_TARGET_LINE "${SOURCE-OF-RESOURCE}"

static autoList_t *SSOR_Sources;

static void SSOR_ReplaceSORLine(void)
{
	autoList_t *lines = readLines(ManualFile);
	uint index;

	index = findLine(lines, SSOR_REPLACE_TARGET_LINE);

	if (index < getCount(lines))
	{
		memFree((void *)desertElement(lines, index));
		InsertLinesToLines(lines, index, SSOR_Sources);
		writeLines(ManualFile, lines);
	}
	releaseDim(lines, 1);
}
static void SetSourceOfResource(void)
{
	autoList_t *files = slssFiles(ResourceDir);
	char *file;
	uint index;

	SSOR_Sources = newList();

	foreach (files, file, index)
	if (!_stricmp("_source.txt", getLocal(file)))
	{
		autoList_t *lines = readLines(file);
		char *line;
		uint line_index;

		if (1 <= getCount(SSOR_Sources))
			addElement(SSOR_Sources, (uint)strx(""));

		foreach (lines, line, line_index)
		{
			uint c;

			for (c = line_index + 1; c; c--)
				line = insertLine(line, 0, "　");

			addElement(SSOR_Sources, (uint)line);
		}
		releaseDim(lines, 0);
	}
	SSOR_ReplaceSORLine();
}

// ----

int main(int argc, char **argv)
{
	char *dir = nextArg();

	LOGPOS();
	errorCase(m_isEmpty(dir));

	dir = makeFullPath(dir);

	errorCase(isRootDirAbs(dir));
	errorCase(!existDir(dir));

	OutDir = dir;
	RootDir = changeLocal(dir, "");
	ResourceDir = combine(RootDir, "Resource");
	ManualFile = combine(OutDir, "Readme.txt");

	errorCase(!existDir(ResourceDir));
	errorCase(!existFile(ManualFile));

	// ----

	LOGPOS();
	SetSourceOfResource();
	LOGPOS();

	// ----
}
