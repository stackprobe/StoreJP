#include "C:\Factory\Common\all.h"

int main(int argc, char **argv)
{
	char *srcFile;
	char *destFile;
	uint count;

	LOGPOS();
	srcFile = nextArg();
	count = toValue(nextArg());
	destFile = nextArg();

	for(; count; count--)
	{
		LOGPOS();
		joinFile(destFile, srcFile);
	}
	LOGPOS();
}
