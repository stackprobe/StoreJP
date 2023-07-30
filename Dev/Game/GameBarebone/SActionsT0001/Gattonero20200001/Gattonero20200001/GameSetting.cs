using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte
{
	public static class GameSetting
	{
		public static I2Size UserScreenSize;
		public static bool FullScreen = false;
		public static bool MouseCursorShow = true;
		public static double MusicVolume;
		public static double SEVolume;

		public const int SAVE_DATA_SLOT_NUM = 10;

		public class SaveDataSlot
		{
			public SimpleDateTime SavedDateTime = SimpleDateTime.FromTimeStamp(19700101000000);
			public string Description = "";
			public string SerializedString = ""; // "" == データ無し
		}

		public static SaveDataSlot[] SaveDataSlots = SCommon.Generate(SAVE_DATA_SLOT_NUM, () => new SaveDataSlot()).ToArray();

		public static void Initialize()
		{
			UserScreenSize = GameConfig.ScreenSize;
			MusicVolume = GameConfig.DefaultMusicVolume;
			SEVolume = GameConfig.DefaultSEVolume;
		}

		private const string SERIALIZED_DATA_START_MARK = "<GameSetting>";
		private const string SERIALIZED_DATA_END_MARK = "</GameSetting>";

		public static string Serialize()
		{
			List<object> dest = new List<object>();

			dest.Add(SERIALIZED_DATA_START_MARK);

			// ---- 保存データここから ----

			dest.Add(UserScreenSize.W);
			dest.Add(UserScreenSize.H);
			dest.Add(FullScreen);
			dest.Add(MouseCursorShow);
			dest.Add(DD.RateToPPB(MusicVolume));
			dest.Add(DD.RateToPPB(SEVolume));

			foreach (Input input in Inputs.GetAllInput())
			{
				dest.Add(input.Key);
				dest.Add(input.Button);
			}
			foreach (SaveDataSlot slot in SaveDataSlots)
			{
				dest.Add(slot.SavedDateTime.ToTimeStamp());
				dest.Add(slot.Description);
				dest.Add(slot.SerializedString);
			}

			// ---- 保存データここまで ----

			dest.Add(SERIALIZED_DATA_END_MARK);

			return SCommon.Serializer.I.Join(dest.Select(v => v.ToString()).ToArray());
		}

		public static void Deserialize(string serializedString)
		{
			string[] src = SCommon.Serializer.I.Split(serializedString);
			int c = 0;

			if (src[c++] != SERIALIZED_DATA_START_MARK)
				throw new Exception("Bad SERIALIZED_DATA_START_MARK");

			// ---- 保存データここから ----

			UserScreenSize.W = SCommon.ToRange(int.Parse(src[c++]), 1, SCommon.IMAX);
			UserScreenSize.H = SCommon.ToRange(int.Parse(src[c++]), 1, SCommon.IMAX);
			FullScreen = bool.Parse(src[c++]);
			MouseCursorShow = bool.Parse(src[c++]);
			MusicVolume = DD.PPBToRate(int.Parse(src[c++]));
			SEVolume = DD.PPBToRate(int.Parse(src[c++]));

			foreach (Input input in Inputs.GetAllInput())
			{
				input.Key = SCommon.ToRange(int.Parse(src[c++]), 0, Keyboard.KEY_MAX - 1);
				input.Button = SCommon.ToRange(int.Parse(src[c++]), 0, Pad.BUTTON_MAX - 1);
			}
			for (int index = 0; index < SAVE_DATA_SLOT_NUM; index++)
			{
				SaveDataSlot slot = SaveDataSlots[index];

				slot.SavedDateTime = SimpleDateTime.FromTimeStamp(long.Parse(src[c++]));
				slot.Description = src[c++];
				slot.SerializedString = src[c++];
			}

			// ---- 保存データここまで ----

			if (src[c++] != SERIALIZED_DATA_END_MARK)
				throw new Exception("Bad SERIALIZED_DATA_END_MARK");
		}
	}
}
