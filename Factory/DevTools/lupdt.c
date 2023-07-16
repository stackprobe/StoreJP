/*
	�w��f�B���N�g�����Ō�ɕҏW���ꂽ������\������B
	�w��f�B���N�g���z���̃t�@�C���ɂ��čŐV�̍X�V������\������B

	----
	�R�}���h

	lupdt.exe [/F] [/L] [�w��f�B���N�g��]

		/F ... �t�@�C�������\������B
		/L ... �����̑S�Ẵf�B���N�g���ɂ��ČʂɎ��s����B
*/

#include "C:\Factory\Common\all.h"

static int ShowFileMode;

static void ShowLastUpdatedTime(char *trailer)
{
	autoList_t *files = slssFiles(".");
	char *file;
	uint index;
	uint64 updatedStamp;
	time_t updatedTime;
	time_t newestUpdatedTime = -1L;

	foreach (files, file, index)
	{
		getFileStamp(file, NULL, NULL, &updatedStamp);
		updatedTime = getTimeByFileStamp(updatedStamp);

		if (newestUpdatedTime < updatedTime)
		{
			newestUpdatedTime = updatedTime;

			if (ShowFileMode)
				cout("%s %s\n", c_makeJStamp(getStampDataTime(newestUpdatedTime), 0), file);
		}
	}
	releaseDim(files, 1);

	errorCase_m(newestUpdatedTime == -1L, "�t�@�C����������܂���B");

	cout("%s", c_makeJStamp(getStampDataTime(newestUpdatedTime), 0));

	if (trailer)
		cout(" %s", trailer);

	cout("\n");
}
static void ShowListMain(void)
{
	autoList_t *dirs = slsDirs(".");
	char *dir;
	uint index;

	foreach (dirs, dir, index)
	{
		addCwd(dir);
		{
			ShowLastUpdatedTime(getLocal(dir));
		}
		unaddCwd();
	}
	releaseDim(dirs, 1);
}
int main(int argc, char **argv)
{
readArgs:
	if (argIs("/F"))
	{
		ShowFileMode = 1;
		goto readArgs;
	}
	if (argIs("/L"))
	{
		if (hasArgs(1))
		{
			addCwd(nextArg());
			{
				ShowListMain();
			}
			unaddCwd();
		}
		else
		{
			ShowListMain();
		}
		return;
	}

	if (hasArgs(1))
	{
		addCwd(nextArg());
		{
			ShowLastUpdatedTime(NULL);
		}
		unaddCwd();
	}
	else
	{
		ShowLastUpdatedTime(NULL);
	}
}
