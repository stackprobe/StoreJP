#include "C:\Factory\Common\all.h"

int main(int argc, char **argv)
{
	coExecute("DvBtHs");

	if (existFile(FOUNDLISTFILE))
		return;

	coExecute("DvUtHs");

	if (existFile(FOUNDLISTFILE))
		return;

	coExecute("DvCmHs");
}
