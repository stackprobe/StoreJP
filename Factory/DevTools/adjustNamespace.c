/*
	adjustNamespace.exe [/B] [変換を行うソースのディレクトリ]

		変換を行うソースのディレクトリ は プロジェクトのルートディレクトリ と同じか配下であること。

		プロジェクトのルートディレクトリ == .csproj があるディレクトリ
*/

#include "C:\Factory\Common\all.h"
#include "C:\Factory\Common\Options\UTF.h"

static int BatchMode;

static char *GetProjFile_Dir(char *dir)
{
	autoList_t *files = lsFiles(dir);
	char *file;
	uint index;
	char *projFile;

	foreach (files, file, index)
		if (!_stricmp("csproj", getExt(file)))
			break;

	if (file)
		projFile = strx(file);
	else
		projFile = NULL;

	releaseDim(files, 1);
	return projFile;
}
static char *GetProjFile(char *dir)
{
	char *projFile;

	dir = strx(dir);

	while (!(projFile = GetProjFile_Dir(dir)))
	{
		errorCase(isRootDirAbs(dir));

		dir = getParent_x(dir);
	}
	memFree(dir);
	return projFile;
}
static char *GetProjRootNamespace(char *projFile)
{
	autoList_t *lines = readLines(projFile);
	char *line;
	uint index;
	char *ret = NULL;

	foreach (lines, line, index)
	{
		ucTrim(line);

		if (startsWith(line, "<RootNamespace>") && endsWith(line, "</RootNamespace>"))
		{
			ret = strx(ne_strchr(line, '>') + 1);
			*ne_strchr(ret, '<') = '\0';
			ret = UTF8ToSJISText_x(ret);
			goto endFunc;
		}
	}
	error(); // not found

endFunc:
	releaseDim(lines, 1);
	return ret;
}

static int NSWithBracket;

static char *GetNamespaceFromLine(char *line)
{
	char *namespace = strx(line + 10);

	if (endsWith(namespace, " {"))
	{
		namespace[strlen(namespace) - 2] = '\0';
		NSWithBracket = 1;
	}
	else
	{
		NSWithBracket = 0;
	}

	namespace = UTF8ToSJISText(namespace);

	return namespace;
}
static char *GetNamespaceFromPath(char *rootNamespace, char *rootDir, char *file)
{
	char *namespace;

	file = changeRoot(strx(file), rootDir, NULL);
	namespace = xcout("%s\\%s", rootNamespace, file);
	namespace = changeLocal_xc(namespace, "");
	replaceChar(namespace, '\\', '.');

	memFree(file);
	return namespace;
}
static char *SetClassNameToClassTop(char *oldClassTop, char *className)
{
	char *newClassTop;
	char *p;
	char *q;
	char *q2;

	p = strstrNext(oldClassTop, "\x20""class\x20"); // クラスの場合

	if (!p)
	{
		p = strstrNext(oldClassTop, "\x20""struct\x20"); // 構造体の場合
		errorCase(!p);
	}
	q  = strchr(p, ':'); // 継承している場合
	q2 = strchr(p, '<'); // ジェネリック型の場合 || 親クラスがジェネリック型の場合

	if (q && q2)
		m_minim(q, q2);
	else
		m_maxim(q, q2);

	newClassTop = strxRng(oldClassTop, p);
	newClassTop = addLine(newClassTop, className);

	if (q)
	{
		if (*q == ':')
			newClassTop = addChar(newClassTop, ' ');

		newClassTop = addLine(newClassTop, q);
	}
	return newClassTop;
}
static void AdjustNamespace(char *targetDir)
{
	char *projFile;
	char *rootDir;
	char *rootNamespace;
	autoList_t *files;
	char *file;
	uint index;

	targetDir = makeFullPath(targetDir);

	cout("T %s\n", targetDir);

	projFile = GetProjFile(targetDir);
	rootDir = getParent(projFile);
	rootNamespace = GetProjRootNamespace(projFile);

	cout("P %s\n", projFile);
	cout("D %s\n", rootDir);
	cout("N %s\n", rootNamespace);

	if (!BatchMode)
	{
		cout("続行？\n");

		if (clearGetKey() == 0x1b)
			termination(0);

		cout("続行します。\n");
	}
	files = lssFiles(targetDir);

	foreach (files, file, index)
	{
		if (!_stricmp("cs", getExt(file)))
		{
			autoList_t *lines = readLines(file);
			char *line;
			uint index;
			int modified = 0;
			char *relFile = changeRoot(strx(file), rootDir, NULL);

			foreach (lines, line, index)
			{
				if (lineExp("namespace <1,,>", line))
				{
					char *oldNamespace = GetNamespaceFromLine(line);
					char *newNamespace = GetNamespaceFromPath(rootNamespace, rootDir, file);

					cout("F %s\n", relFile);
					cout("< %s\n", oldNamespace);
					cout("> %s\n", newNamespace);

					if (strcmp(oldNamespace, newNamespace))
					{
						LOGPOS();
						memFree(line);
						line = xcout("namespace %s%s", newNamespace, NSWithBracket ? " {" : "");
						line = SJISToUTF8Text_x(line);
						setElement(lines, index, (uint)line);
						modified = 1;
					}
					memFree(oldNamespace);
					memFree(newNamespace);
				}
				// インデント1段 + public --> このソースファイルのクラス・構造体の最初の1行と見なす。
				if (lineExp("\tpublic <1,,>", line))
				{
					char *oldClassTop = UTF8ToSJISText(line);
					char *newClassTop;
					char *className;

					className = changeExt(getLocal(relFile), "");
					newClassTop = SetClassNameToClassTop(oldClassTop, className);

					cout("F %s\n", relFile);
					cout("C %s\n", className);
					cout("< %s\n", oldClassTop);
					cout("> %s\n", newClassTop);

					if (strcmp(oldClassTop, newClassTop))
					{
						LOGPOS();
						memFree(line);
						line = SJISToUTF8Text(newClassTop);
						setElement(lines, index, (uint)line);
						modified = 1;
					}
					memFree(oldClassTop);
					memFree(newClassTop);
					memFree(className);
				}
			}
			if (modified)
			{
				LOGPOS();
				semiRemovePath(file);
				writeLines(file, lines);
			}
			releaseDim(lines, 1);
			memFree(relFile);
		}
	}
	LOGPOS();
	releaseDim(files, 1);
	memFree(targetDir);
	memFree(projFile);
	memFree(rootDir);
	memFree(rootNamespace);
	LOGPOS();
}
int main(int argc, char **argv)
{
	if (argIs("/B"))
	{
		BatchMode = 1;
	}

	if (hasArgs(1))
	{
		AdjustNamespace(nextArg());
		return;
	}

	{
		char *targetDir;

		cout("[C#]変換を行うソースのディレクトリ：\n");
		targetDir = dropDir();

		AdjustNamespace(targetDir);

		memFree(targetDir);
	}
}
