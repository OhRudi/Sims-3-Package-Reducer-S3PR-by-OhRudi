using System;
using System.Collections.Generic;

namespace s3molib
{
	public static class Helper
	{
		public static string UInt32ToHexString(uint num)
		{
			string s = Convert.ToString((long)((ulong)num), 16).ToUpper();
			s = s.PadLeft(8, '0');
			return "0x" + s;
		}

		public static string UInt64ToHexString(ulong num)
		{
			ulong num3 = (ulong)((uint)((num & 18446744069414584320UL) >> 32));
			uint num2 = (uint)(num & (ulong)uint.MaxValue);
			string s = Convert.ToString((long)num3, 16).ToUpper();
			s = s.PadLeft(8, '0');
			string s2 = Convert.ToString((long)((ulong)num2), 16).ToUpper();
			s2 = s2.PadLeft(8, '0');
			return "0x" + s + s2;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Helper()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["0x00AE6C67"] = "BONE";
			dictionary["0x00B2D882"] = "_IMG";
			dictionary["0x00B552EA"] = "_SPT";
			dictionary["0x015A1849"] = "GEOM";
			dictionary["0x0166038C"] = "NMAP";
			dictionary["0x01661233"] = "MODL";
			dictionary["0x01A527DB"] = "_AUD";
			dictionary["0x01D0E6FB"] = "VBUF";
			dictionary["0x01D0E70F"] = "IBUF";
			dictionary["0x01D0E723"] = "VRTG";
			dictionary["0x01D0E75D"] = "MATD";
			dictionary["0x01D0E76B"] = "SKIN";
			dictionary["0x01D10F34"] = "MLOD";
			dictionary["0x01EEF63A"] = "_AUD";
			dictionary["0x02019972"] = "MTST";
			dictionary["0x021D7E8C"] = "SPT2";
			dictionary["0x0229684B"] = "VBUF";
			dictionary["0x0229684F"] = "";
			dictionary["0x022B756C"] = "";
			dictionary["0x025C90A6"] = "_CSS";
			dictionary["0x025C95B6"] = "LAYO";
			dictionary["0x025ED6F4"] = "SIMO";
			dictionary["0x029E333B"] = "VOCE";
			dictionary["0x02C9EFF2"] = "MIXR";
			dictionary["0x02D5DF13"] = "JAZZ";
			dictionary["0x02DC343F"] = "OBJK";
			dictionary["0x033260E3"] = "TKMK";
			dictionary["0x0333406C"] = "_XML";
			dictionary["0x033A1435"] = "TXTC";
			dictionary["0x033B2B66"] = "";
			dictionary["0x0341ACC9"] = "TXTF";
			dictionary["0x034AEECB"] = "CASP";
			dictionary["0x0354796A"] = "TONE";
			dictionary["0x03555BA8"] = "TONE";
			dictionary["0x0355E0A6"] = "BOND";
			dictionary["0x0358B08A"] = "FACE";
			dictionary["0x03B33DDF"] = "ITUN";
			dictionary["0x03B4C61D"] = "LITE";
			dictionary["0x03D843C2"] = "CCHE";
			dictionary["0x03D86EA4"] = "DETL";
			dictionary["0x03E80CDC"] = "";
			dictionary["0x0418FE2A"] = "CFEN";
			dictionary["0x044AE110"] = "COMP";
			dictionary["0x046A7235"] = "";
			dictionary["0x048A166D"] = "UNKN";
			dictionary["0x0498DA7E"] = "";
			dictionary["0x049CA4CD"] = "CSTR";
			dictionary["0x04A09283"] = "UNKN";
			dictionary["0x04A4D951"] = "WDET";
			dictionary["0x04AC5D93"] = "CPRX";
			dictionary["0x04B30669"] = "CTTL";
			dictionary["0x04C58103"] = "CRAL";
			dictionary["0x04D82D90"] = "CMRU";
			dictionary["0x04ED4BB2"] = "CTPT";
			dictionary["0x04EE6ABB"] = "UNKN";
			dictionary["0x04F3CC01"] = "CFIR";
			dictionary["0x04F51033"] = "SBNO";
			dictionary["0x04F66BCC"] = "UNKN";
			dictionary["0x04F88964"] = "SIME";
			dictionary["0x051DF2DD"] = "CBLN";
			dictionary["0x05512255"] = "UNKN";
			dictionary["0x0553EAD4"] = "UNKN";
			dictionary["0x0563919E"] = "UNKN";
			dictionary["0x0580A2B4"] = "";
			dictionary["0x0580A2B5"] = "";
			dictionary["0x0580A2B6"] = "";
			dictionary["0x0580A2CD"] = "SNAP";
			dictionary["0x0580A2CE"] = "SNAP";
			dictionary["0x0580A2CF"] = "SNAP";
			dictionary["0x0580A2B4"] = "THUM";
			dictionary["0x0580A2B5"] = "THUM";
			dictionary["0x0580A2B6"] = "THUM";
			dictionary["0x0589DC44"] = "THUM";
			dictionary["0x0589DC45"] = "THUM";
			dictionary["0x0589DC46"] = "THUM";
			dictionary["0x0591B1AF"] = "UPST";
			dictionary["0x05B17698"] = "THUM";
			dictionary["0x05B17699"] = "THUM";
			dictionary["0x05B1769A"] = "THUM";
			dictionary["0x05B1B524"] = "THUM";
			dictionary["0x05B1B525"] = "THUM";
			dictionary["0x05B1B526"] = "THUM";
			dictionary["0x05CD4BB3"] = "";
			dictionary["0x05DA8AF6"] = "WBND";
			dictionary["0x05E4FAF7"] = "UNKN";
			dictionary["0x05ED1226"] = "REFS";
			dictionary["0x05FF3549"] = "UNKN";
			dictionary["0x05FF6BA4"] = "2ARY";
			dictionary["0x0604ABDA"] = "DMTR";
			dictionary["0x060B390C"] = "CWAT";
			dictionary["0x060E1826"] = "UNKN";
			dictionary["0x0611B0E7"] = "UNKN";
			dictionary["0x062853A8"] = "FAMD";
			dictionary["0x062C8204"] = "BBLN";
			dictionary["0x062E9EE0"] = "";
			dictionary["0x06302271"] = "CINF";
			dictionary["0x063261DA"] = "HINF";
			dictionary["0x06326213"] = "OBCI";
			dictionary["0x06393F5D"] = "UNKN";
			dictionary["0x06393F5D"] = "UNKN";
			dictionary["0x065BFCAC"] = "";
			dictionary["0x065BFCAD"] = "";
			dictionary["0x065BFCAE"] = "";
			dictionary["0x0668F628"] = "";
			dictionary["0x0668F630"] = "";
			dictionary["0x0668F635"] = "TWNI";
			dictionary["0x0668F639"] = "TWNP";
			dictionary["0x067CAA11"] = "BGEO";
			dictionary["0x06B981ED"] = "OBJS";
			dictionary["0x06CE4804"] = "META";
			dictionary["0x06D6B112"] = "UNKN";
			dictionary["0x06DC847E"] = "UNKN";
			dictionary["0x073FAA07"] = "S3SA";
			dictionary["0x07046B39"] = "";
			dictionary["0x07CD07EC"] = "";
			dictionary["0x0A36F07A"] = "CCFP";
			dictionary["0x0C37A5B5"] = "LOOK";
			dictionary["0x0C07456D"] = "COLL";
			dictionary["0x11E32896"] = "";
			dictionary["0x16B17A6C"] = "";
			dictionary["0x1F886EAD"] = "_INI";
			dictionary["0x220557DA"] = "STBL";
			dictionary["0x2AD195F2"] = "";
			dictionary["0x2653E3C8"] = "THUM";
			dictionary["0x2653E3C9"] = "THUM";
			dictionary["0x2653E3CA"] = "THUM";
			dictionary["0x2D4284F0"] = "THUM";
			dictionary["0x2D4284F1"] = "THUM";
			dictionary["0x2D4284F2"] = "THUM";
			dictionary["0x2E75C764"] = "ICON";
			dictionary["0x2E75C765"] = "ICON";
			dictionary["0x2E75C766"] = "ICON";
			dictionary["0x2E75C767"] = "ICON";
			dictionary["0x2F7D0002"] = "IMAG";
			dictionary["0x2F7D0004"] = "IMAG";
			dictionary["0x2F7D0008"] = "UITX";
			dictionary["0x312E7545"] = "UNKN";
			dictionary["0x316C78F2"] = "CFND";
			dictionary["0x319E4F1D"] = "OBJD";
			dictionary["0x32C83095"] = "";
			dictionary["0x342778A7"] = "";
			dictionary["0x342779A7"] = "";
			dictionary["0x34E5247C"] = "UNKN";
			dictionary["0x35A33E29"] = "";
			dictionary["0x3A65AF29"] = "MINF";
			dictionary["0x3D8632D0"] = "";
			dictionary["0x4D1A5589"] = "OBJN";
			dictionary["0x4F09F8E1"] = "UNKN";
			dictionary["0x515CA4CD"] = "CWAL";
			dictionary["0x54372472"] = "TSNP";
			dictionary["0x5DE9DBA0"] = "THUM";
			dictionary["0x5DE9DBA1"] = "THUM";
			dictionary["0x5DE9DBA2"] = "THUM";
			dictionary["0x626F60CC"] = "THUM";
			dictionary["0x626F60CD"] = "THUM";
			dictionary["0x626F60CE"] = "THUM";
			dictionary["0x628A788F"] = "";
			dictionary["0x63A33EA7"] = "ANIM";
			dictionary["0x6ABFAD26"] = "UNKN";
			dictionary["0x6B20C4F3"] = "CLIP";
			dictionary["0x6B6D837D"] = "SNAP";
			dictionary["0x6B6D837E"] = "SNAP";
			dictionary["0x6B6D837F"] = "SNAP";
			dictionary["0x72683C15"] = "STPR";
			dictionary["0x736884F1"] = "VPXY";
			dictionary["0x73E93EEB"] = "_XML";
			dictionary["0x7672F0C5"] = "";
			dictionary["0x8070223D"] = "AUDT";
			dictionary["0x82B43584"] = "";
			dictionary["0x8EAF13DE"] = "_RIG";
			dictionary["0x8FFB80F6"] = "_ADS";
			dictionary["0x90620000"] = "";
			dictionary["0x90624C1B"] = "";
			dictionary["0x9063660D"] = "WTXT";
			dictionary["0x9063660E"] = "";
			dictionary["0x913381F2"] = "UNKN";
			dictionary["0x9151E6BC"] = "CWST";
			dictionary["0x91EDBD3E"] = "CRST";
			dictionary["0x94C5D14A"] = "SIGR";
			dictionary["0x94EC4B54"] = "UNKN";
			dictionary["0xA2377025"] = "";
			dictionary["0xA5F9FE18"] = "UNKN";
			dictionary["0xA8D58BE5"] = "SKIL";
			dictionary["0xAE39399F"] = "";
			dictionary["0xB074ACE6"] = "";
			dictionary["0xB125533A"] = "UNKN";
			dictionary["0xB1422971"] = "UNKN";
			dictionary["0xB1CC1AF6"] = "_VID";
			dictionary["0xB4DD716B"] = "_INV";
			dictionary["0xB52F5055"] = "FBLN";
			dictionary["0xCF84EC98"] = "";
			dictionary["0xCF9A4ACE"] = "MDLR";
			dictionary["0xD063545B"] = "LDES";
			dictionary["0xD3044521"] = "RSLT";
			dictionary["0xD382BF57"] = "FTPT";
			dictionary["0xD4D9FBE5"] = "PTRN";
			dictionary["0xD84E7FC5"] = "ICON";
			dictionary["0xD84E7FC6"] = "ICON";
			dictionary["0xD84E7FC7"] = "ICON";
			dictionary["0xD9BD0909"] = "";
			dictionary["0xDC37E964"] = "";
			dictionary["0xDD3223A7"] = "BUFF";
			dictionary["0xDEA2951C"] = "PETB";
			dictionary["0xEA5118B0"] = "_SWB";
			dictionary["0xF0633989"] = "";
			dictionary["0xF0FF5598"] = "";
			dictionary["0xF12E5E12"] = "UNKN";
			dictionary["0xF1EDBD86"] = "CRMT";
			dictionary["0xF3A38370"] = "NGMP";
			dictionary["0xF609FD60"] = "";
			dictionary["0xFCEAB65B"] = "THUM";
			Helper.TagList = dictionary;
			Helper.ImageTypes = new List<string>
			{
				"0x0580A2B4",
				"0x0580A2B5",
				"0x0580A2B6",
				"0x0580A2CD",
				"0x0580A2CE",
				"0x0580A2CF",
				"0x0589DC44",
				"0x0589DC45",
				"0x0589DC46",
				"0x0589DC47",
				"0x05B17698",
				"0x05B17699",
				"0x05B1769A",
				"0x05B1B524",
				"0x05B1B525",
				"0x05B1B526",
				"0x0668F635",
				"0x2653E3C8",
				"0x2653E3C9",
				"0x2653E3CA",
				"0x2D4284F0",
				"0x2D4284F1",
				"0x2D4284F2",
				"0x2E75C764",
				"0x2E75C765",
				"0x2E75C766",
				"0x2E75C767",
				"0x2F7D0002",
				"0x2F7D0004",
				"0x5DE9DBA0",
				"0x5DE9DBA1",
				"0x5DE9DBA2",
				"0x626F60CC",
				"0x626F60CD",
				"0x626F60CE",
				"0x6B6D837D",
				"0x6B6D837E",
				"0x6B6D837F",
				"0xAD366F95",
				"0xAD366F96",
				"0xD84E7FC5",
				"0xD84E7FC6",
				"0xD84E7FC7",
				"0xFCEAB65B"
			};

			Helper.THUMResources = new List<uint>
			{
                0x0580A2B4,
				0x0580A2B5,
				0x0580A2B6,
				0x0589DC44,
				0x0589DC45,
				0x0589DC46,
				0x05B17698,
				0x05B17699,
				0x05B1769A,
				0x05B1B524,
				0x05B1B525,
				0x05B1B526,
				0x2653E3C8,
				0x2653E3C9,
				0x2653E3CA,
				0x2D4284F0,
				0x2D4284F1,
				0x2D4284F2,
				0x5DE9DBA0,
				0x5DE9DBA1,
				0x5DE9DBA2,
				0x626F60CC,
				0x626F60CD,
				0x626F60CE,
				0xFCEAB65B
            };

			Helper.ICONResources = new List<uint>
			{
                0x2E75C764,
				0x2E75C765,
				0x2E75C766,
				0x2E75C767,
				0xD84E7FC5,
				0xD84E7FC6,
				0xD84E7FC7
            };

            Helper.DDSTypes = new List<string>
			{
				"0x8FFB80F6",
				"0x00B2D882"
			};
		}

		public static Dictionary<string, string> TagList;

		public static List<string> ImageTypes;

		public static List<uint> THUMResources;

        public static List<uint> ICONResources;

        public static List<string> DDSTypes;
	}
}
