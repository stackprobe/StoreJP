using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.TActions.Enemies;
using Charlotte.Games.TActions.Tiles;
using Charlotte.Games.TActions.Walls;

namespace Charlotte.Games.TActions
{
	// memo:
	// 慣習的な呼び方：
	// マップデータのセルの集合として(将棋盤のように)見た場合
	// -- 全体を「マップ」または「テーブル」、個々のマス目を「タイル」と呼ぶ。
	// ドット単位の座標で見たとき
	// -- 全体を「フィールド」と呼ぶ。

	// memo:
	// 用語集：
	// 4方向_テンキー方式      -- { 2, 4, 6, 8 } の値をとり、それぞれ { 下, 左, 右, 上 } を意味する。
	// 4方向+0vec_テンキー方式 -- { 2, 4, 5, 6, 8 } の値をとり、それぞれ { 下, 左, 0vec(零ベクトル), 右, 上 } を意味する。
	// 8方向_テンキー方式      -- { 1, 2, 3, 4, 6, 7, 8, 9 } の値をとり、それぞれ { 左下, 下, 右下, 左, 右, 左上, 上, 右上 } を意味する。
	// 8方向+0vec_テンキー方式 -- { 1, 2, 3, 4, 5, 6, 7, 8, 9 } の値をとり、それぞれ { 左下, 下, 右下, 左, 0vec(零ベクトル), 右, 左上, 上, 右上 } を意味する。

	/// <summary>
	/// フィールド
	/// マップ情報
	/// </summary>
	public class TAField
	{
		/// <summary>
		/// フィールド名
		/// -- リソースフォルダ直下の Field フォルダ直下の *.txt の「拡張子を除く」ローカルファイル名
		/// </summary>
		public string Name;

		/// <summary>
		/// フィールドを生成する。
		/// フィールド名：
		/// -- リソースフォルダ直下の Field フォルダ直下の *.txt の「拡張子を除く」ローカルファイル名
		/// </summary>
		/// <param name="name">フィールド名</param>
		public TAField(string name)
		{
			if (!SCommon.IsFairLocalPath(name, -1))
				throw new Exception("Bad name");

			this.Name = name;
		}

		#region Load, Save

		private const string FIELD_RES_FILE_PREFIX = "Field\\TA\\";
		private const string FIELD_RES_FILE_SUFFIX = ".txt";

		public void Load()
		{
			byte[] data = DD.GetResFileData(FIELD_RES_FILE_PREFIX + this.Name + FIELD_RES_FILE_SUFFIX).Data.Value;
			string text = SCommon.ENCODING_SJIS.GetString(data);
			string[] lines = SCommon.TextToLines(text)
				.Select(line => line.Trim())
				.Where(line => line != "" && line[0] != ';') // 空行とコメント行を除去
				.ToArray();

			int c = 0;

			this.Table_W = int.Parse(lines[c++]);
			this.Table_H = int.Parse(lines[c++]);

			int TABLE_W_MIN = (GameConfig.ScreenSize.W + TAConsts.TILE_W - 1) / TAConsts.TILE_W;
			int TABLE_H_MIN = (GameConfig.ScreenSize.H + TAConsts.TILE_H - 1) / TAConsts.TILE_H;

			if (this.Table_W < TABLE_W_MIN || SCommon.IMAX < this.Table_W) throw null;
			if (this.Table_H < TABLE_H_MIN || SCommon.IMAX < this.Table_H) throw null;

			this.Table = new TATile[this.Table_W, this.Table_H];
			this.TileNameTable = new string[this.Table_W, this.Table_H];
			this.EnemyNameTable = new string[this.Table_W, this.Table_H];

			for (int x = 0; x < this.Table_W; x++)
			{
				for (int y = 0; y < this.Table_H; y++)
				{
					string tileName = SCommon.RefElement(lines, c++, TATileCatalog.DEFAULT_TILE_NAME);
					string enemyName = SCommon.RefElement(lines, c++, TAEnemyCatalog.DEFAULT_ENEMY_NAME);

					TATile tile;

					if (tileName == TATileCatalog.DEFAULT_TILE_NAME)
						tile = DefaultTile;
					else
						tile = TATileCatalog.I.CreateTile(tileName);

					this.Table[x, y] = tile;
					this.TileNameTable[x, y] = tileName;
					this.EnemyNameTable[x, y] = enemyName;
				}
			}
			this.Description = SCommon.RefElement(lines, c++, "Default");
			this.WallProperty = SCommon.RefElement(lines, c++, TAWallCatalog.I.GetNames()[0]);
			this.MusicProperty = SCommon.RefElement(lines, c++, TAMusicCatalog.I.GetNames()[0]);
			//this.Dummy_990001 = SCommon.RefElement(lines, c++, "Default");
			//this.Dummy_990002 = SCommon.RefElement(lines, c++, "Default");
			//this.Dummy_990003 = SCommon.RefElement(lines, c++, "Default");		
		}

		public void Save()
		{
			List<string> lines = new List<string>();

			lines.Add("" + this.Table_W);
			lines.Add("" + this.Table_H);

			for (int x = 0; x < this.Table_W; x++)
			{
				for (int y = 0; y < this.Table_H; y++)
				{
					string tileName = this.TileNameTable[x, y];
					string enemyName = this.EnemyNameTable[x, y];

					lines.Add(tileName);
					lines.Add(enemyName);
				}
			}
			lines.Add("");
			lines.Add("; +---------------------------------------------------------------------+");
			lines.Add("; | 注意：このファイルにコメントを書いても次のマップ保存時に除去される。|");
			lines.Add("; +---------------------------------------------------------------------+");
			lines.Add("");
			lines.Add("; Description");
			lines.Add(this.Description);
			lines.Add("");
			lines.Add("; WallProperty -- TAWallCatalog.GetNames()");
			lines.Add(this.WallProperty);
			lines.Add("");
			lines.Add("; MusicProperty -- TAMusicCatalog.GetNames()");
			lines.Add(this.MusicProperty);

			string text = SCommon.LinesToText(lines);
			byte[] data = SCommon.ENCODING_SJIS.GetBytes(text);

			DD.SetResFileData(FIELD_RES_FILE_PREFIX + this.Name + FIELD_RES_FILE_SUFFIX, data);
		}

		#endregion

		// マップ(テーブル)の大きさ
		//
		public int Table_W { get; private set; }
		public int Table_H { get; private set; }

		// フィールドの大きさ(ドット単位)
		//
		public int W { get { return this.Table_W * TAConsts.TILE_W; } }
		public int H { get { return this.Table_H * TAConsts.TILE_H; } }

		/// <summary>
		/// マップ(テーブル)
		/// </summary>
		private TATile[,] Table;

		/// <summary>
		/// デフォルトのタイル
		/// -- フィールド(マップ)の外側・何もないタイルとして使用する。
		/// </summary>
		private static TATile DefaultTile = new TATile_Grass();

		/// <summary>
		/// タイルを取得する。
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>タイル</returns>
		public TATile GetTile(I2Point tilePt)
		{
			// ? テーブル(マップ)の外側
			if (
				tilePt.X < 0 || this.Table_W <= tilePt.X ||
				tilePt.Y < 0 || this.Table_H <= tilePt.Y
				)
				return DefaultTile;

			return this.Table[tilePt.X, tilePt.Y];
		}

		/// <summary>
		/// タイルの名前テーブル
		/// 用途：保存・編集
		/// </summary>
		public string[,] TileNameTable;

		/// <summary>
		/// 敵の名前テーブル
		/// 用途：敵の再配置・保存・編集
		/// </summary>
		public string[,] EnemyNameTable;

		/// <summary>
		/// 敵の配置(再配置)
		/// </summary>
		public void ReloadEnemies()
		{
			TAGame.I.Enemies.Clear();

			for (int x = 0; x < this.Table_W; x++)
			{
				for (int y = 0; y < this.Table_H; y++)
				{
					string enemyName = this.EnemyNameTable[x, y];

					if (enemyName != TAEnemyCatalog.DEFAULT_ENEMY_NAME)
					{
						I2Point tilePt = new I2Point(x, y);
						D2Point enemyPt = TACommon.ToFieldPoint(tilePt);
						TAEnemy enemy = TAEnemyCatalog.I.CreateEnemy(enemyName, enemyPt.X, enemyPt.Y);

						TAGame.I.Enemies.Add(enemy);
					}
				}
			}
		}

		/// <summary>
		/// この場所についての説明(この場所の名前)
		/// </summary>
		public string Description;

		/// <summary>
		/// プロパティ：背景
		/// </summary>
		public string WallProperty;

		/// <summary>
		/// プロパティ：音楽
		/// </summary>
		public string MusicProperty;
	}
}
