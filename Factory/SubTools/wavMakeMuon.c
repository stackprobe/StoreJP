/*
	wavVolume.exe (/S SEC | /M MILLIS) [WAV-FILE]
*/

#include "C:\Factory\Common\all.h"

#define HZ 44100

static void MakeMuonWav(uint millis, char *outputFile)
{
	char *wavFile = makeTempPath(NULL);
	char *csvFile = makeTempPath(NULL);
	FILE *csvFp;
	char *hzFile = makeTempPath(NULL);
	uint sampling;
	uint count;

	errorCase(IMAX / HZ < millis);
	errorCase(m_isEmpty(outputFile));

	cout("< %u millis\n", millis);
	cout("> %s\n", outputFile);

	sampling = (millis * HZ) / 1000;

	csvFp = fileOpen(csvFile, "wt");

	for (count = sampling; count; count--)
		writeLine(csvFp, "32768,32768");

	fileClose(csvFp);

	writeOneLine_cx(hzFile, xcout("%u", HZ));

	coExecute_x(xcout("C:\\Factory\\SubTools\\wavCsv.exe /C2W \"%s\" \"%s\" \"%s\"", csvFile, hzFile, wavFile));

	errorCase_m(!existFile(wavFile), "wavCsv.exe error !!!");

	copyFile(wavFile, outputFile);

	removeFile_x(wavFile);
	removeFile_x(csvFile);
	removeFile_x(hzFile);
}
static void Main2(uint millis)
{
	if (hasArgs(1))
	{
		char *outputFile = makeFullPath(nextArg());

		errorCase_m(hasArgs(1), "不明なコマンド引数(2)");

		MakeMuonWav(millis, outputFile);

		memFree(outputFile);
	}
	else
	{
		char *outputDir = makeFreeDir();
		char *outputFile;

		outputFile = combine_cx(outputDir, xcout("muon%us.wav", millis / 1000));

		MakeMuonWav(millis, outputFile);

		execute_x(xcout("START %s", outputDir));

		memFree(outputDir);
		memFree(outputFile);
	}
}
int main(int argc, char **argv)
{
	if (argIs("/S"))
	{
		uint sec = toValue(nextArg());

		Main2(sec * 1000);
		return;
	}
	if (argIs("/M"))
	{
		uint millis = toValue(nextArg());

		Main2(millis);
		return;
	}
	error_m("不明なコマンド引数");
}
