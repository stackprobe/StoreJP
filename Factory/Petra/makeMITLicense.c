#include "C:\Factory\Common\all.h"

#define LICENSE_TEMPLATE "C:\\Factory\\Resource\\MITLicenseTemplate.txt"

static uint GetYearFrom(char *license)
{
	char *p;
	uint y;

	errorCase(strlen(license) < 6);

	for (p = license; p[5]; p++)
	{
		if (
			!m_isdecimal(p[0]) &&
			 m_isdecimal(p[1]) && p[1] != '0' &&
			 m_isdecimal(p[2]) &&
			 m_isdecimal(p[3]) &&
			 m_isdecimal(p[4]) &&
			!m_isdecimal(p[5])
			)
		{
			y = toValue_x(strxl(p + 1, 4));
			goto endFunc;
		}
	}
	error();

endFunc:
	return y;
}
static int IsOutFileChanged(char *outFile, char *newBinText)
{
	if (existFile(outFile))
	{
		char *oldBinText = readText_b(outFile);
		int changed;
		changed = strcmp(oldBinText, newBinText);
		memFree(oldBinText);

		if (!changed)
			return 0;
	}
	return 1;
}
int main(int argc, char **argv)
{
	char *outFile = makeFullPath(nextArg());
	char *template = readText_b(LICENSE_TEMPLATE);
	char *strYear;
	char *license;
	uint y1;
	uint y2 = (uint)(toValue64_x(makeCompactStamp(NULL)) / 10000000000UL);

	cout("> %s\n", outFile);
	LOGPOS();

	errorCase(y2 < 1000 || 9999 < y2); // ? not 4Œ…

	if (existFile(outFile))
	{
		char *oldLicense = readText_b(outFile);

		y1 = GetYearFrom(oldLicense);

		memFree(oldLicense);
	}
	else
	{
		y1 = y2;
	}

	if (y1 == y2)
	{
		strYear = xcout("%u", y1);
	}
	else
	{
		strYear = xcout("%u-%u", y1, y2);
	}

	license = strx(template);
	license = replaceLine(license, "${YEAR}", strYear, 0);

	if (IsOutFileChanged(outFile, license))
	{
		writeOneLineNoRet_b(outFile, license);
	}

	memFree(outFile);
	memFree(template);
	memFree(strYear);
	memFree(license);

	LOGPOS();
}
