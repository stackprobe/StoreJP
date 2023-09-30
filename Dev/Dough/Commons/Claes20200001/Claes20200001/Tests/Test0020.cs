using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.UTF8Conv.ToJString テスト
	/// </summary>
	public class Test0020
	{
		public void Test01()
		{
			#region Resource

			string RES_TEXT = @"

日本の歴史は古代から現代に至るまで、多くの重要な出来事と文化の発展を経てきました。最古の時代、弥生時代から古墳時代にかけて、農耕と鉄器の使用が広まり、日本の社会が発展しました。
その後、奈良時代には仏教の伝来と国家の確立が行われ、平安時代には京都が首都として栄えました。また、鎌倉時代には武士階級が力をつけ、日本は幕府制度が確立されました。
室町時代には文化が栄え、茶道や能楽が発展しましたが、戦国時代には国内が戦乱の時代となりました。その後、江戸時代には平和な時代が訪れ、江戸幕府の下で国内が統一されました。
19世紀には外国との接触が増え、明治維新によって封建制度が廃止され、近代化が進行しました。日本は帝国主義政策を展開し、第一次世界大戦後には国際的な地位を高めましたが、第二次世界大戦で敗北し、占領下におかれました。
しかし、占領下での民主化と復興により、日本は戦後急速に発展し、経済大国として世界で成功を収めました。現代においても、日本は先進国としての地位を保ち、独自の文化と伝統を守りつつ、国際社会で重要な役割を果たしています。

Japanese history has witnessed significant developments and cultural achievements from ancient times to the present day. During the Yayoi and Kofun periods, Japan saw the spread of agriculture and the use of iron tools, leading to social advancements.
Subsequently, in the Nara period, Buddhism was introduced, and the foundation of the state was established. The Heian period saw Kyoto prosper as the capital. During the Kamakura period, the samurai class gained power, leading to the establishment of shogunate rule.
The Muromachi period was marked by cultural flourishing, with the development of tea ceremonies and Noh theater. However, the Sengoku period brought domestic turmoil and conflict. Later, the Edo period ushered in an era of peace and unification under the Tokugawa Shogunate.
In the 19th century, Japan increased its contact with foreign nations, leading to the Meiji Restoration, which abolished feudalism and initiated modernization. Japan pursued imperialist policies, gaining international prominence after World War I but ultimately facing defeat and occupation after World War II.
However, during the post-war period, Japan underwent rapid democratization and reconstruction, emerging as an economic powerhouse. Today, Japan maintains its status as an advanced nation, preserving its unique culture and traditions while playing a significant role in the international community.

世界の歴史は多くの時代と出来事によって形成されています。古代エジプトやメソポタミアでは、文明の萌芽が見られ、巨大なピラミッドや粘土板の記録が残されました。
古代ギリシャでは哲学、芸術、政治が栄え、デモクラシーの概念が生まれました。そして、古代ローマ帝国はその影響力を世界中に広げ、法律とインフラストラクチャーの発展をもたらしました。
中世ヨーロッパでは、キリスト教の宗教的影響が強まり、騎士道や封建制度が根付きました。一方、東アジアでは唐や宋の中国、日本の平安時代が文化的な黄金時代を迎えました。
近代に入り、ルネサンスと産業革命がヨーロッパで起こり、科学と技術の進歩が世界を変えました。また、大航海時代により新大陸が発見され、世界は地理的にも拡大しました。
19世紀には帝国主義と国際紛争が激化し、世界大戦が勃発。その後、国際連盟の設立や国際協力の試みが行われましたが、第二次世界大戦が勃発し、世界中に甚大な影響を及ぼしました。
現代において、国際連合が設立され、冷戦後の多くの国々が民主主義を採用し、国際社会での協力と紛争解決が重要です。世界の歴史は継続的な進化と変化の過程であり、過去の経験から学びつつ、未来に向かって前進しています。

World history has been shaped by numerous epochs and events. In ancient Egypt and Mesopotamia, the seeds of civilization were sown, leaving behind colossal pyramids and records on clay tablets.
Ancient Greece saw the flourishing of philosophy, art, and politics, giving birth to the concept of democracy. Meanwhile, the Roman Empire extended its influence globally, contributing to the development of laws and infrastructure.
Medieval Europe witnessed the rise of Christianity, along with chivalry and feudalism taking root. In East Asia, China's Tang and Song dynasties and Japan's Heian period experienced cultural golden ages.
Entering the modern era, the Renaissance and Industrial Revolution in Europe triggered significant advancements in science and technology, reshaping the world. The Age of Exploration led to the discovery of new continents, expanding the world geographically.
The 19th century brought about intensified imperialism and international conflicts, eventually culminating in two world wars. Subsequently, the establishment of the League of Nations and attempts at international cooperation took place, but World War II had a profound global impact.
In the contemporary era, the United Nations was founded, and many nations adopted democracy after the end of the Cold War. Cooperation and conflict resolution on the international stage have become crucial. World history represents an ongoing process of evolution and change, where we learn from past experiences while moving forward into the future.

";

			#endregion

			string baseText = RES_TEXT.Trim();

			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				string text = baseText;

				text = text.Substring(SCommon.CRandom.GetInt(text.Length + 1));
				text = text.Substring(0, SCommon.CRandom.GetInt(text.Length + 1));

				byte[] bytes = Encoding.UTF8.GetBytes(text);

				if (SCommon.CRandom.GetBoolean()) // 50 Pct
				{
					bytes = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(bytes).ToArray(); // BOM を付ける。
				}

				int c_end = SCommon.CRandom.GetInt(text.Length + 1);
				for (int c = 0; c < c_end; c++)
				{
					bytes[SCommon.CRandom.GetInt(bytes.Length)] ^= (byte)(1 << SCommon.CRandom.GetInt(8));
				}

				string retText = SCommon.UTF8Conv.ToJString(bytes);

				//Console.WriteLine(retText);

				// ----

				if (retText.Replace("\r", "") != SCommon.ToJString(retText, true, true, true, true))
					throw null; // BUG
			}
			Console.WriteLine("OK! (TEST-0020-01)");
		}

		public void Test02()
		{
			Test02_a("\r\n\t\u0020" + SCommon.ASCII + SCommon.GetJChars());
			Test02_a("日本語\rの間に\n制御コ\tードと 空　白");

			Console.WriteLine("OK! (TEST-0020-02)");
		}

		private void Test02_a(string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			string retText = SCommon.UTF8Conv.ToJString(bytes);

			//Console.WriteLine(text);
			//Console.WriteLine(retText);

			if (retText != text)
				throw null; // BUG
		}
	}
}
