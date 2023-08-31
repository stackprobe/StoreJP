﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Utilities
{
	public class CsvFileWriter : IDisposable
	{
		public const char DELIMITER_COMMA = ',';
		public const char DELIMITER_TAB = '\t';

		private char Delimiter = DELIMITER_COMMA;
		private StreamWriter Writer;

		public CsvFileWriter(string file, bool append = false)
			: this(file, append, SCommon.ENCODING_SJIS)
		{ }

		public CsvFileWriter(string file, bool append, Encoding encoding)
			: this(file, append, encoding, DELIMITER_COMMA)
		{ }

		public CsvFileWriter(string file, bool append, Encoding encoding, char delimiter)
			: this(new StreamWriter(file, append, encoding))
		{
			this.Delimiter = delimiter;
		}

		public CsvFileWriter(StreamWriter writer_binding)
		{
			this.Writer = writer_binding;
		}

		/// <summary>
		/// 次に書き込むセルが行の最初のセルか
		/// </summary>
		private bool FirstCell = true;

		public void WriteCell(string cell, bool forceQuote = false)
		{
			if (this.FirstCell)
				this.FirstCell = false;
			else
				this.Writer.Write(this.Delimiter);

			if (
				forceQuote ||
				cell.Contains('"') ||
				cell.Contains('\n') ||
				cell.Contains(this.Delimiter)
				)
			{
				this.Writer.Write('"');
				this.Writer.Write(cell.Replace("\"", "\"\""));
				this.Writer.Write('"');
			}
			else
			{
				this.Writer.Write(cell);
			}
		}

		public void EndRow()
		{
			this.Writer.Write('\n');
			this.FirstCell = true;
		}

		public void WriteCells(IList<string> cells)
		{
			foreach (string cell in cells)
			{
				this.WriteCell(cell);
			}
		}

		public void WriteRow(IList<string> row)
		{
			foreach (string cell in row)
			{
				this.WriteCell(cell);
			}
			this.EndRow();
		}

		public void WriteRows(IList<IList<string>> rows)
		{
			foreach (IList<string> row in rows)
			{
				this.WriteRow(row);
			}
		}

		public void Dispose()
		{
			if (this.Writer != null)
			{
				this.Writer.Dispose();
				this.Writer = null;
			}
		}
	}
}
