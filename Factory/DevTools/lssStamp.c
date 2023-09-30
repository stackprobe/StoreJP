/*
	lssStamp.exe ����

	lssStamp.exe �쐬���� �ŏI�X�V����

	lssStamp.exe �쐬���� �ŏI�A�N�Z�X���� �ŏI�X�V����

	----
	�g�p��

	lssStamp.exe 20230916213325000

	lssStamp.exe 20230916213311000 20230916213322000

	lssStamp.exe 20230916213301000 20230916213302000 20230916213303000
*/

#include "C:\Factory\Common\all.h"

static uint64 ArgToStamp(char *arg)
{
	errorCase_m(!lineExp("<17,09>", arg), "Bad time-stamp arg (not YYYYMMDDHHIISSLLL)");

	return toValue64(arg);
}
static void DoAllSetStamp(uint64 createStamp, uint64 accessStamp, uint64 updateStamp)
{
	autoList_t *files = readLines(FOUNDLISTFILE);
	char *file;
	uint index;

	LOGPOS();

	foreach (files, file, index)
	{
		errorCase_m(!existFile(file), "no file");

		/*
			�s���ȓ����� setFileStamp() ���� error() �ɂ��Ă����͂��B
		*/
		setFileStamp(file, createStamp, accessStamp, updateStamp);
	}
	LOGPOS();

	releaseDim(files, 1);
}
int main(int argc, char **argv)
{
	if (hasArgs(2))
	{
		uint64 createStamp;
		uint64 accessStamp;
		uint64 updateStamp;

		createStamp = ArgToStamp(nextArg());
		accessStamp = ArgToStamp(nextArg());
		updateStamp = ArgToStamp(nextArg());

		DoAllSetStamp(createStamp, accessStamp, updateStamp);
		return;
	}
	if (hasArgs(2))
	{
		uint64 createStamp;
		uint64 updateStamp;

		createStamp = ArgToStamp(nextArg());
		updateStamp = ArgToStamp(nextArg());

		DoAllSetStamp(createStamp, m_max(createStamp, updateStamp), updateStamp);
		return;
	}
	if (hasArgs(1))
	{
		uint64 stamp;

		stamp = ArgToStamp(nextArg());

		DoAllSetStamp(stamp, stamp, stamp);
		return;
	}
}
