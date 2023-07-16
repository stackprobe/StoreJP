using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.TActions.Enemies;

namespace Charlotte.Games.TActions.Fields
{
	public class TAField_Test0001 : TAField
	{
		#region Resource

		private static readonly string RES_MAPS = @"

＠＠＠＠＠～～～～～～～～～～～～～～～～～～～～～～～～～～～～＠＠＠＠＠＠＠＠＠＠・・・＠＠＠＠
＠＠＠＠＠＠～～～～～～～～～～～～～～～～～～～～～～～～～～～＠＠＠＠＠＠＠・・＠・・・＠・・＠
＠＠＠＠＠＠～～～～～～～＠＠～～～～～～～～～～～～～～～～～～＠＠＠＠＠＠＠・・＠・・・＠・・＠
＠＠＠＠＠＠＠＠～～～～＠＠＠＠＠＠＠＠～～～～～～～～～～～～・・＠＠＠＠・＠＠＠＠・・・＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠～～～～～～～～～～～・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠～～～～～～～・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠～～～～～～・・・・・・・・・・・・・・・・・・＠
＠＠＠＠・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠～～～・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠・・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠・＠・・・＠＠・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・＠＠・・・・・・・・・・・・・・・＠
＠・・・・・・・・・＠・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・＠＠＠＠＠＠＠＠＠＠・・・＠・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・＠＠＠＠＠＠＠＠＠・・＠＠・・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・＠＠＠＠＠＠＠＠・・＠・・＠・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・＠・・・＠・・・・・・・・・・・・・・・・・～～＠・＠～～・・・・・＠
＠＠＠・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・～・・・・・～・・・・・＠
＠＠＠・・・・・・・・・・・・・・・・＠・・・・・・・～～～～～～・・・・～・＠＠＠・～・・・・・＠
＠＠＠・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・～・・・・～・＠＠＠・～・・・・・＠
＠＠＠・・・・・・・・・・・・＠・・・＠＠＠・・・・・・～・～・～・・・・～・・・・・～・・・・・＠
＠＠＠＠・・・・・・・・・・・・・・・・・＠・・・・・・～～～～～・・・・～～～～～～～・・・・・＠
＠＠＠＠・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠・・・・・・・・・・・＠＠・・・＠＠＠・・・・・～～～～～～～～・・・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・・・＠＠＠＠・・・・・・～～～～～～～～～～～・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・～～～～～～～～～～～～～～～・・・・・・・＠
＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・～～・・・・～～～～～～～～～・・・・・・＠
＠＠＠・・・・～～～・・・・・・・・・・・・・・・・・・・・・・・・・・～～～～～～～・・・・・・＠
＠＠＠・・・・～～～・・・・・・・・・・・・・・・・・・・・・・・・・・・～～～～～～・・・・・・＠
＠＠・・・・・～～～・・・・・・・・＠＠・・・・・・・・・・・・・・・・・・～～～・・・・・・・・＠
＠＠・・・・・・・・・・・・・・・＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠・・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・・＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠・・・・・・・・・・・・・＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠・・・・・・・・・・・・＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠・・＠
＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠・＠＠
＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠・・＠＠
＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠・＠＠＠
＠＠＠＠＠・・・＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠・・＠＠＠
＠＠＠＠＠・・・＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠・・・＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠

/

～～～～～～～～～～～～～～～～～～～～～～～～～～～＠＠・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～～～～～～～～～＠＠＠＠＠～～～～～～～～～～～＠＠・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～～・・・・・＠＠・・・・・＠＠・・・・・・・・・＠＠・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～～・・・・＠・・・・・・・・・＠・・・・・・・・＠＠・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～～・・・・＠・・・・・・・・・・＠・・・・・・・＠＠・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～・・・・＠・・・・・・・・・・・＠・・・・・・・＠＠・・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～・・・・＠・・・・・・・・・・・・＠・・・・・・＠＠・・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～～・・・・＠・・・・・・・・・・・・＠・・・・・・＠＠・・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～～・・・・・＠＠・・・・・・・・・・・＠・・・・・・＠＠・・・・・・・＠＠＠＠＠＠＠＠＠＠・・＠＠
～～・・・・・＠＠・・・・・・・・・・・＠・・・・・・＠＠・・・・・・・・・・＠＠＠＠＠＠・・・・＠
～～・・・・・・＠・・・・・・・・・・・＠・・・・・・＠＠・・・・・・・・・・・・・・・・・・・・・
＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・＠・・・・・・＠＠・・・・・・・・・・・・・・・・・・・・・
・・・～～・・・＠＠・・・・・・・・・＠・・・・・＠＠＠＠・・・・・・・・・・・・・・・・・・・・・
・・・～～・・・・＠・・・・・・・・＠・・・・＠＠＠～～＠・・・・・・・・・・・・・・・・・・・・・
・・・～～・・・・・・・・・・・・・＠・・・・＠～～～～＠・・・・・・・・・・・・・・＠＠＠＠＠＠＠
・・・～～・・・・＠・・・・・・・・＠・・・＠＠～～～＠＠・・・・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠・・・・＠＠＠＠・＠～～～～＠・・・・・・・＠～～～～～～＠・＠＠＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・＠＠＠＠＠＠＠～～～＠＠・・・・・・・＠＠＠～～～～＠・・＠＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・＠＠＠＠＠＠＠～～～＠・＠＠＠・＠＠＠・・＠～～～～＠・・・＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠・・・・＠＠＠＠・＠～～～＠・＠・・・・・＠・・＠～～～～＠・・・・＠＠＠
＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・＠～～～＠・＠・・・・・＠・・＠～～～～＠・・・・・＠＠
＠・・・・・・・・・＠・・・・・・・・・・・＠～～～＠・＠・・・・・＠・・＠～～～～＠・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠・＠・・・・・＠・・＠＠＠＠＠＠・・・・・～～
＠・・・・・・・・・＠・・・・・・・・・・・・・・・・・＠＠＠・＠＠＠・・・・・・・・・・・・～～～
＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・～～～
＠・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・～～～
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・～～～
＠・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・～～～
＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・～～～
＠・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・～～～
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠・＠・・・・・・・・・・・・・・・～～
＠・・・・・・・・・＠・・・・・・・・・＠＠＠＠＠・・＠＠＠＠・＠＠＠＠＠＠＠＠・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・＠・・・・・・・・・・・・・・・・・・＠・・・・・・・・＠＠
＠・・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・＠・・・・・・・＠＠＠
＠・・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠＠＠＠＠＠
＠・・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠
＠・・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠
＠・・・・・・・・・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠
＠・・・＠・・・＠・・・・・・・・・・・＠・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠
＠・・・・・・・・・・・・・・・・～～～＠・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠
＠・・・・・・・・・・・・・・・～～～～～～～・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠
＠・・・・・・・・・・・・・・・～～～～～～～～～・・・・・・・・・・・・・・・・・・・・・・・＠＠
＠・・・・・・・＠・・・・・・～～～～～～～～～～～・・・・・・・・・・・・・・・・・・・・・・＠＠
＠＠・・・・・・・・・・・・～～～～～～～～～～～～～・・・・・・・・・・・・・・・・・・・・・＠＠
＠＠＠・・・・・・・・・・・～～～～～～～～～～～～～～・・・・・・・・・・・・・・・・・・・・＠＠
＠＠＠＠・・・・・・・・・・～～～～～～～～～～～～～～・・・・・＠＠＠＠・・・・・・・・・・・＠＠
＠＠＠＠＠・・・・・・・・～～～～～～～～～～～～～～～～・・・＠＠＠＠＠＠・・・・・・・・・・＠＠
＠＠＠＠＠＠＠＠＠＠＠＠・～～～～～～～～～～～～～～～～・・＠＠＠＠＠＠＠＠・・・・・・・・・＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠～～～～～～～～～～～～～～～～・＠＠＠＠＠＠＠＠＠＠・・・・・・・・＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠～～～～～～～～～～～～～～～～＠＠＠＠＠＠＠＠＠＠＠＠・・・・・・・＠＠

/

＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠・・・・・・・・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠＠＠・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠

/

＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
＠＠＠＠＠・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠＠
＠＠＠＠・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠
＠＠＠・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠
＠＠・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠
＠・・・・・・・・・・・・・・・・・・・・＠・＠・・・＠＠＠
・・・・・・・・・・・・・・・・・・・＠・・＠・・＠・・＠＠
・・・・・・・・・・・・・・・・・・・・＠・・・＠・・・・＠
・・・＠＠＠＠＠・・・・・・・・・・＠・・・・・・・＠・・＠
＠＠＠＠＠＠＠＠＠・・・・・・・・・・＠・・・・・＠・・・＠
＠＠＠＠＠＠＠＠＠＠・・・・・・・・＠・・・・・・・＠・・＠
＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・＠・・・＠・・・・＠
＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・＠・・・・・＠・・・＠
＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠・・・・・・・・・・・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・・＠＠＠＠＠＠
＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠・・・＠＠＠＠＠＠
＠＠＠＠＠＠＠＠・・・・・・・・＠＠・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠・・・・・・・・＠＠・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠・・・・・・・・＠＠・・・・・・・・・・・＠
＠＠＠＠＠＠＠＠・・・＠＠・・・＠＠・・・＠＠＠＠＠＠＠＠＠
＠＠＠＠＠＠＠＠・・・＠＠・・・＠＠・・・＠＠＠＠＠＠＠＠＠
～～～～～～～～・・・＠＠・・・・・・・・・・・・・・・・＠
～～～～～～～～・・・＠＠・・・・・・・・・・・・・・・・＠
～・・・・・～～・・・＠＠・・・・・・・・・・・・・・・・＠
～・＠＠＠・～～・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～・＠・＠・～～・・・＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠
～・＠・＠・～～・・・・・・・・・・・・・・・・・・・・・＠
～・・・・・～～・・・・・・・・・・・・・・・・・・・・・＠
～・・・・・～～・・・・・・・・・・・・・・・・・・・・・＠
～・・・・・～～・・・・・・・・・＠・・・＠・・・・・・・＠
～・・・・・～～・・・・・・・・＠＠・・・＠＠・・・・・・＠
～・・・・・～～～～～～～～～～～～・・・～～～～～～～～＠
～・・・・・～～～～～～～～～～～～・・・～～～～～～～～＠
～・・・・・・・・・・・・・・・＠＠・・・＠＠・・・・・＠＠
～・・・・・・・・・・・・・・・・＠・・・＠・・・・・＠＠＠
～・・・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠
～～・・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠
～～・・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠
～～～・・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠＠
～～～～～・・・・・・・・・・・・・・・・・・・・＠＠＠＠＠
～～～～～～～～・・・・・・・・～～・・・・～～・・・～～＠
～～～～～～～～～～～～・・・～～～～・・～～～～～～～～＠
～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～
～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～

";

		#endregion

		public static TAField_Test0001 Create(int index)
		{
			string[] lines = SCommon.TextToLines(SCommon.Tokenize(RES_MAPS, "/")[index].Trim());

			int w = lines[0].Length;
			int h = lines.Length;

			return new TAField_Test0001(w, h, lines, index);
		}

		private static int CharToTileValue(char chr)
		{
			int value = "・～＠".IndexOf(chr);

			if (value == -1)
				throw null;

			return value;
		}

		private int[,] Table;
		private int FieldIndex;

		private TAField_Test0001(int w, int h, string[] lines, int index)
			: base(new I2Size(w, h))
		{
			int[,] table = new int[w, h];

			for (int x = 0; x < w; x++)
				for (int y = 0; y < h; y++)
					table[x, y] = CharToTileValue(lines[y][x]);

			this.Table = table;
			this.FieldIndex = index;
		}

		public override void Initialize()
		{
			TAGame.I.Player.X = this.W / 2.0;
			TAGame.I.Player.Y = this.H / 2.0;

			Action<int, Func<int, bool>, Func<int, I2Point>> setStartPoint = (end, check, i2TPt) =>
			{
				int imin = -1;
				int imax = -1;

				for (int i = 0; i < end; i++)
				{
					if (check(i))
					{
						if (imin == -1)
							imin = i;

						imax = i;
					}
				}
				if (imin == -1)
					throw null; // never

				D2Point startPoint = TACommon.ToFieldPoint((i2TPt(imin) + i2TPt(imax)) / 2);

				TAGame.I.Player.X = startPoint.X;
				TAGame.I.Player.Y = startPoint.Y;
			};

			switch (TAGameMaster.I.IntoDirection)
			{
				case 4: setStartPoint(this.Table_H, i => this.Table[0, i] == 0, i => new I2Point(0, i)); break;
				case 6: setStartPoint(this.Table_H, i => this.Table[this.Table_W - 1, i] == 0, i => new I2Point(this.Table_W - 1, i)); break;
				case 8: setStartPoint(this.Table_W, i => this.Table[i, 0] == 0, i => new I2Point(i, 0)); break;
				case 2: setStartPoint(this.Table_W, i => this.Table[i, this.Table_H - 1] == 0, i => new I2Point(i, this.Table_H - 1)); break;

				default:
					break;
			}
			switch (TAGameMaster.I.IntoDirection)
			{
				case 4:
				case 6:
				case 8:
				case 2:
					//TAGame.I.Player.FaceDirection = 10 - TAGameMaster.I.IntoDirection; // 侵入方向
					TAGame.I.Player.FaceDirection = TAGameMaster.I.LastFaceDirection; // 最後に向いていた方向
					break;

				default:
					break;
			}
			for (int c = 0; c < 10; c++)
			{
				D2Point pt = new D2Point(
					(double)(int)SCommon.CRandom.GetDoubleRange(50.0, TAGame.I.Field.W - 150.0),
					(double)(int)SCommon.CRandom.GetDoubleRange(50.0, TAGame.I.Field.H - 150.0)
					);

				if (DD.GetDistance(pt, new D2Point(TAGame.I.Player.X, TAGame.I.Player.Y)) < 200.0) // ? プレイヤーに近すぎる。
					continue;

				TAGame.I.Enemies.Add(new TAEnemy_Test0001(pt.X, pt.Y));
			}
			Musics.SunBeams.Play();
		}

		public override TAField GetNextField(int direction)
		{
			int index;

			// マップ配置：
			// +---+---+
			// | 2 |   |
			// +---+---+
			// | 1 | 3 |
			// +---+---+
			// | 0 |   |
			// +---+---+

			switch (this.FieldIndex * 10 + direction)
			{
				case 8: index = 1; break;
				case 16: index = 3; break;
				case 18: index = 2; break;
				case 12: index = 0; break;
				case 22: index = 1; break;
				case 34: index = 1; break;

				default:
					throw null; // never
			}
			return TAField_Test0001.Create(index);
		}

		protected override int P_GetTile(I2Point tilePt)
		{
			return this.Table[tilePt.X, tilePt.Y];
		}

		protected override void P_DrawTile(I2Point tilePt, D2Point drawPt)
		{
			DD.Draw(Pictures.Grass, drawPt);

			if (this.IsRiver(tilePt))
			{
				bool stranger8 = !this.IsRiver1x1(tilePt.X, tilePt.Y - 1);
				bool stranger2 = !this.IsRiver1x1(tilePt.X, tilePt.Y + 1);
				bool stranger4 = !this.IsRiver1x1(tilePt.X - 1, tilePt.Y);
				bool stranger6 = !this.IsRiver1x1(tilePt.X + 1, tilePt.Y);

				bool stranger1 = !this.IsRiver1x1(tilePt.X - 1, tilePt.Y + 1);
				bool stranger3 = !this.IsRiver1x1(tilePt.X + 1, tilePt.Y + 1);
				bool stranger7 = !this.IsRiver1x1(tilePt.X - 1, tilePt.Y - 1);
				bool stranger9 = !this.IsRiver1x1(tilePt.X + 1, tilePt.Y - 1);

				int mode_lt;
				int mode_rt;
				int mode_rb;
				int mode_lb;

				if (stranger4 && stranger8)
				{
					mode_lt = 0;
				}
				else if (stranger4)
				{
					mode_lt = 1;
				}
				else if (stranger8)
				{
					mode_lt = 2;
				}
				else if (stranger7)
				{
					mode_lt = 3;
				}
				else
				{
					mode_lt = 4;
				}

				if (stranger6 && stranger8)
				{
					mode_rt = 0;
				}
				else if (stranger6)
				{
					mode_rt = 1;
				}
				else if (stranger8)
				{
					mode_rt = 2;
				}
				else if (stranger9)
				{
					mode_rt = 3;
				}
				else
				{
					mode_rt = 4;
				}

				if (stranger6 && stranger2)
				{
					mode_rb = 0;
				}
				else if (stranger6)
				{
					mode_rb = 1;
				}
				else if (stranger2)
				{
					mode_rb = 2;
				}
				else if (stranger3)
				{
					mode_rb = 3;
				}
				else
				{
					mode_rb = 4;
				}

				if (stranger4 && stranger2)
				{
					mode_lb = 0;
				}
				else if (stranger4)
				{
					mode_lb = 1;
				}
				else if (stranger2)
				{
					mode_lb = 2;
				}
				else if (stranger1)
				{
					mode_lb = 3;
				}
				else
				{
					mode_lb = 4;
				}

				int koma = (DD.ProcFrame / 4) % 8;

				DD.Draw(Pictures.River[0, mode_lt * 2 + 0, koma], drawPt + new D2Point(-8, -8));
				DD.Draw(Pictures.River[1, mode_rt * 2 + 0, koma], drawPt + new D2Point(8, -8));
				DD.Draw(Pictures.River[1, mode_rb * 2 + 1, koma], drawPt + new D2Point(8, 8));
				DD.Draw(Pictures.River[0, mode_lb * 2 + 1, koma], drawPt + new D2Point(-8, 8));
			}
			else if (this.IsWall(tilePt))
			{
				bool drawed = false;

				if ((tilePt.X + tilePt.Y) % 2 == 0)
				{
					if (this.IsWall2x2(tilePt.X - 1, tilePt.Y - 1))
					{
						DD.Draw(Pictures.Tree[4], drawPt);
						drawed = true;
					}
					if (this.IsWall2x2(tilePt.X, tilePt.Y))
					{
						DD.Draw(Pictures.Tree[1], drawPt);
						drawed = true;
					}
				}
				else
				{
					if (this.IsWall2x2(tilePt.X, tilePt.Y - 1))
					{
						DD.Draw(Pictures.Tree[2], drawPt);
						drawed = true;
					}
					if (this.IsWall2x2(tilePt.X - 1, tilePt.Y))
					{
						DD.Draw(Pictures.Tree[3], drawPt);
						drawed = true;
					}
				}
				if (!drawed)
				{
					DD.Draw(Pictures.Tree[0], drawPt);
				}
			}
		}

		private bool IsRiver1x1(int x, int y)
		{
			return this.IsRiverOrOut(new I2Point(x, y));
		}

		private bool IsRiverOrOut(I2Point tilePt)
		{
			return this.IsRiver(tilePt) || this.IsOut(tilePt);
		}

		private bool IsWall2x2(int x, int y)
		{
			return
				this.IsWallOrOut(new I2Point(x + 0, y + 0)) &&
				this.IsWallOrOut(new I2Point(x + 0, y + 1)) &&
				this.IsWallOrOut(new I2Point(x + 1, y + 0)) &&
				this.IsWallOrOut(new I2Point(x + 1, y + 1));
		}

		private bool IsWallOrOut(I2Point tilePt)
		{
			return this.IsWall(tilePt) || this.IsOut(tilePt);
		}

		protected override IEnumerable<bool> E_DrawWall()
		{
			for (; ; )
			{
				// none

				yield return true;
			}
		}
	}
}
