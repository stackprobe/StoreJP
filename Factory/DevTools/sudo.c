/*
	sudo.exe COMMAND [OPTION...]

	----

	sudo.exe DIR > 1.txt とやると sudo の標準出力を 1.txt にリダイレクトしてしまうので
	sudo.exe DIR ">" 1.txt とやれば DIR をリダイレクトできるよ。

	指定コマンドはバッチの１行として実行されるよ！
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
	sleep(500); // バッチの終了を待つ。
	LOGPOS();
	removeFile(batFile);
	removeFile(batFile_02);

	memFree(batFile);
	memFree(batFile_02);
	memFree(homeDirAndRunWkDir);
	releaseDim(commandAndOptions, 0);
	memFree(strEvBatEnd);
}
