/*
	sudo.exe COMMAND [OPTION...]

	----

	sudo.exe DIR > 1.txt �Ƃ��� sudo �̕W���o�͂� 1.txt �Ƀ��_�C���N�g���Ă��܂��̂�
	sudo.exe DIR ">" 1.txt �Ƃ��� DIR �����_�C���N�g�ł����B

	�w��R�}���h�̓o�b�`�̂P�s�Ƃ��Ď��s������I
*/

#include "C:\Factory\Common\all.h"
#include "C:\Factory\Common\Options\CRandom.h"

int main(int argc, char **argv)
{
	char *batFile    = makeTempPath("bat");
	char *batFile_02 = makeTempPath("bat");
	char *homeDirAndRunWkDir = getCwd();
	autoList_t *commandAndOptions = allArgs();
	char *strEvBatEnd = MakeUUID(1);
	uint evBatEnd;

	errorCase_m(getCount(commandAndOptions) < 1, "no command");

	evBatEnd = eventOpen(strEvBatEnd);

	{
		FILE *fp = fileOpen(batFile, "wt");

		writeLine_x(fp, xcout("CD /D \"%s\"", homeDirAndRunWkDir));
		writeLine_x(fp, xcout("CALL \"%s\"", batFile_02));
		writeLine_x(fp, xcout("C:\\Factory\\SubTools\\EventSet.exe %s", strEvBatEnd));

		fileClose(fp);
	}

	{
		FILE *fp = fileOpen(batFile_02, "wt");
		char *token;
		uint index;

		foreach (commandAndOptions, token, index)
		{
			int hasSp = 0;
			char *p;

			for (p = token; *p && !hasSp; p++)
				if (*p <= ' ')
					hasSp = 1;

			if (index)
				writeChar(fp, ' ');

			if (hasSp)
				writeChar(fp, '\"');

			writeToken(fp, token);

			if (hasSp)
				writeChar(fp, '\"');
		}
		writeChar(fp, '\n');
		fileClose(fp);
	}

	coExecute_x(xcout("TYPE %s", batFile));    // cout
	coExecute_x(xcout("TYPE %s", batFile_02)); // cout

	coExecute_x(xcout("PowerShell.exe Start-Process \"%s\" -Verb runas -WindowStyle Minimized", batFile));
	changeCwd(homeDirAndRunWkDir);
	LOGPOS();
	eventSleep(evBatEnd);
	LOGPOS();
	sleep(500); // �o�b�`�̏I����҂B
	LOGPOS();
	removeFile(batFile);
	removeFile(batFile_02);

	memFree(batFile);
	memFree(batFile_02);
	memFree(homeDirAndRunWkDir);
	releaseDim(commandAndOptions, 0);
	memFree(strEvBatEnd);
}
