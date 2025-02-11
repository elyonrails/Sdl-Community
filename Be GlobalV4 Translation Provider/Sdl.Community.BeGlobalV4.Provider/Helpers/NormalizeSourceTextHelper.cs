﻿using System.Globalization;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class NormalizeSourceTextHelper
	{
		public string GetCorespondingLangCode(CultureInfo cultureInfo)
		{
			if (cultureInfo != null)
			{
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("zho"))
				{
					//Chinese (Traditional, Macao S.A.R.),(Traditional, Hong Kong SAR),(Traditional, Taiwan)
					if (cultureInfo.Name.Equals("zh-MO") || cultureInfo.Name.Equals("zh-HK") || cultureInfo.Name.Equals("zh-TW"))
					{
						return "cht";
					}
					//Simplified Chinese
					return "chi";
				}
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("deu"))
				{
					return "ger";
				}
				//Language code for Dutch in BeGlobal is dut
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("nld"))
				{
					return "dut";
				}
				return cultureInfo.ThreeLetterISOLanguageName;
			}
			return string.Empty;
		}
	}
}
