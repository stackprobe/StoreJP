#include "C:\Factory\Common\all.h"

static void RemoveAllComment(void)
{
	autoList_t *files = lssFiles(".");
	char *file;
	uint index;

	foreach (files, file, index)
	{
		autoList_t *lines = readLines(file);
		char *line;
		uint line_index;

		foreach (lines, line, line_index)
		{
			if (lineExp("<\t\t>//<>", line))
			{
				memFree(line);
				setElement(lines, line_index, 0);
			}
		}
		removeZero(lines);

		writeLines_cx(file, lines);
	}
	releaseDim(files, 1);
}
int main(int argc, char **argv)
{
	char *targetDir = makeFullPath(nextArg()); // g

	LOGPOS();
	errorCase(!existDir(targetDir));

	addCwd(targetDir);
	{
		RemoveAllComment();
	}
	unaddCwd();
	LOGPOS();
}
