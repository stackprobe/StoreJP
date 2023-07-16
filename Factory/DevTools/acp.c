/*
	acp.exe ���̓t�@�C�� �o�̓t�@�C��

		���̓t�@�C��, �o�̓t�@�C�� ... �p�X���� *P ���J�����gDIR�̃��[�J����(�v���W�F�N�g��)�ɒu��������B
			�v���W�F�N�g���ɓ��t���t���Ă���Ώ�������B

		���[�gDIR������s����ƃv���W�F�N�g���𓾂��Ȃ����߃G���[�ɂȂ�B
*/

#include "C:\Factory\Common\all.h"

#define REP_PTN_PROJECT_NAME "*P"

int main(int argc, char **argv)
{
	char *rFile;
	char *wFile;
	char *projectName = getLocal(getCwd()); // g

	errorCase(m_isEmpty(projectName));

	if (lineExp("<8,09>_<1,,>", projectName)) // ? ���t���t���Ă���B-> ��������B
		projectName += 9;

	LOGPOS();
	cout("P %s\n", projectName);

	rFile = makeFullPath(nextArg());
	wFile = makeFullPath(nextArg());

	LOGPOS();

	rFile = replaceLine(rFile, REP_PTN_PROJECT_NAME, projectName, 0);
	wFile = replaceLine(wFile, REP_PTN_PROJECT_NAME, projectName, 0);

	LOGPOS();
	cout("< %s\n", rFile);
	cout("> %s\n", wFile);

	copyFile(rFile, wFile);

	LOGPOS();

	memFree(rFile);
	memFree(wFile);

	LOGPOS();
}
