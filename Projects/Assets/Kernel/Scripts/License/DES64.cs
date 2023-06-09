using System;
using System.Runtime.InteropServices;

public class DES64
{
    //TODO: change MAX_TEXT_LEN  
	int MAX_TEXT_LEN = 8;

    byte []IP = new byte[64]{57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,
			61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7,
			56,48,40,32,24,16,8,0,58,50,42,34,26,18,10,2,
			60,52,44,36,28,20,12,4,62,54,46,38,30,22,14,6};
    byte []IP1 = new byte[64];
    byte []EP = new byte[48]{31,0,1,2,3,4,3,4,5,6,7,8,
			    7,8,9,10,11,12,11,12,13,14,15,16,
			    15,16,17,18,19,20,19,20,21,22,23,24,
			    23,24,25,26,27,28,27,28,29,30,31,0};
    byte [][][]SBOX = new byte[][][]{new byte[][]{new byte[]{14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7},
					    new byte[16]{0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8},
					    new byte[16]{4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0},
					    new byte[16]{15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13}},

					    new byte[4][]{new byte[16]{15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10},
					    new byte[16]{3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5},
					    new byte[16]{0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15},
					    new byte[16]{13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9}},

					    new byte[4][]{new byte[16]{10, 0,  9, 14, 6,  3, 15, 5,  1, 13, 12, 7, 11,4,2,8},
					    new byte[16]{13,7,0,9,3,  4,  6, 10, 2,  8,  5, 14, 12, 11, 15, 1},
					    new byte[16]{13,6,4,9,8,15, 3,  0, 11, 1,  2, 12, 5, 10, 14, 7},
					    new byte[16]{1,10,13,0,6,9,  8,  7,  4, 15, 14, 3, 11, 5,  2, 12}},
					
					    new byte[4][]{new byte[16]{7, 13, 14 ,3,  0,  6,  9, 10, 1,  2,  8,  5, 11, 12, 4, 15},
					    new byte[16]{13, 8, 11, 5,  6, 15, 0,  3,  4,  7,  2, 12, 1, 10, 14, 9},
					    new byte[16]{10, 6,  9,  0, 12, 11, 7, 13, 15, 1,  3, 14, 5,  2,  8,  4},
					    new byte[16]{3, 15, 0 , 6, 10, 1 ,13 ,8 , 9,  4,  5, 11, 12, 7,  2, 14}},
					
					    new byte[4][]{new byte[16]{2, 12, 4,  1,  7, 10, 11, 6,  8,  5,  3, 15, 13, 0, 14, 9},
					    new byte[16]{ 14, 11, 2, 12, 4 , 7, 13, 1,  5,  0, 15, 10, 3,  9,  8,  6},
					    new byte[16]{ 4,  2,  1, 11, 10, 13, 7,  8, 15, 9, 12, 5 , 6,  3,  0, 14},
					    new byte[16]{ 11, 8, 12, 7,  1, 14, 2, 13, 6, 15, 0,  9, 10, 4,  5,  3}},
										
					    new byte[4][]{new byte[16]{12, 1, 10 ,15, 9,  2,  6,  8,  0, 13, 3,  4 ,14, 7,  5, 11},
					    new byte[16]{10, 15, 4 , 2,  7, 12, 9,  5,  6,  1 ,13, 14, 0 ,11, 3,  8},
					    new byte[16]{9, 14, 15, 5,  2,  8, 12, 3,  7,  0,  4, 10, 1, 13, 11, 6},
					    new byte[16]{4,  3,  2, 12, 9,  5, 15, 10, 11, 14, 1,  7,  6,  0,  8, 13}},
										
					    new byte[4][]{new byte[16]{4, 11, 2, 14, 15, 0,  8, 13, 3, 12, 9,  7,  5, 10, 6,  1},
					    new byte[16]{13, 0, 11, 7,  4,  9,  1, 10, 14, 3,  5, 12, 2, 15, 8,  6},
					    new byte[16]{1,  4, 11, 13, 12, 3,  7, 14, 10, 15, 6,  8,  0,  5,  9,  2},
					    new byte[16]{6, 11, 13, 8,  1,  4, 10, 7,  9,  5,  0 ,15, 14, 2,  3, 12}},
										
					    new byte[4][]{new byte[16]{13, 2,  8,  4,  6, 15, 11 ,1, 10, 9,  3, 14, 5,  0, 12, 7},
					    new byte[16]{1, 15, 13, 8, 10, 3,  7,  4 ,12, 5,  6, 11, 0, 14, 9,  2},
					    new byte[16]{7,  11,  4,  1,  9, 12, 14, 2,  0,  6, 10, 13, 15, 3,  5, 8},
					    new byte[16]{2, 1, 14, 7,  4, 10, 8, 13, 15, 12, 9,  0,  3,  5,  6, 11}}};
    byte []P = new byte[32]{15,6,19,20,28,11,27,16,0,14,22,25,4,17,30,9,
			    1,7,23,13,31,26,2,8,18,12,29,5,21,10,3,24};
    byte []PC1 = new byte[56]{56,48,40,32,24,16,8,0,57,49,41,33,25,17,
			      9,1,58,50,42,34,26,18,10,2,59,51,43,35,
			    62,54,46,38,30,22,14,6,61,53,45,37,29,21,
			    13,5,60,52,44,36,28,20,12,4,27,19,11,3};
    byte []PC2 = new byte[48]{13,16,10,23,0,4,2,27,15,5,20,9,
			    22,18,11,3,25,7,15,6,26,19,12,1,
			    40,51,30,36,46,54,29,39,50,44,32,47,
			    43,48,38,55,33,52,45,41,49,35,28,31};
    byte []LS = new byte[16]{1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1};

    public DES64()
    {
	    for (int nIndexI=0;nIndexI<64;nIndexI++)
            IP1[IP[nIndexI]]= (byte)nIndexI;
    }
#if UNITY_EDITOR
    //------------------------
    //function name		: Encrypt
    //parameter			: strText[IN]	;string to encrypt
    //						  strKey[IN]	;key string
    //return				; encrypted string 
    //------------------------
    public string Encrypt(string strText, byte[] strKey)
    {
        int nLength;
        int nPlainTextLen = strText.Length;
        if (nPlainTextLen % 8 == 0) nLength = nPlainTextLen;
        else nLength = (nPlainTextLen / 8 + 1) * 8;

        if (nLength > MAX_TEXT_LEN)
        {
            return "";
        }
        string chPlainText = "";
        byte[] nEnCode = new byte[MAX_TEXT_LEN];
        string chResult = "";

        chPlainText = strText;
        textToEnByteArr(chPlainText, strKey, nEnCode);
        chResult = enByteArrToHexStr(nEnCode, chResult, MAX_TEXT_LEN);
        return chResult;
    }
#endif
    //------------------------
    //function name		: Decrypt
    //parameter			: strText[IN]	;string to decrypt
    //						  strKey[IN]	;key string
    //return				; decrypted string 
    //------------------------
    public string Decrypt(string strEnText, byte[] strKey)
    {
	    int nLength = strEnText.Length;
	    if (nLength / 2 > MAX_TEXT_LEN)
	    {
		    return "";
	    }

		byte[] nEnCode = new byte[MAX_TEXT_LEN + 1];
        string chResult= "";
		char []strText = new char[MAX_TEXT_LEN]; 
	    hexStrToByteArr(ref nEnCode, strEnText, nLength);
        
		byteArrToDeText(ref strText, strKey, nEnCode, nLength / 2);
		for(int k = 0; k < strText.Length; k++)
		{
			if (strText[k] != '\0')
				chResult += strText[k];
		}
	    return chResult;
    }
    void keySchedule(byte []_chKey, byte [][]_chRoundKey)
    {
	    byte []ch64BitKey = new byte[64];
		byte []chTmpKey = new byte[56];
		byte []chC = new byte[28];
		byte []chD = new byte[28];
	    int nIndexI,nIndexJ;

	    for (nIndexI=0;nIndexI<64;nIndexI++) ch64BitKey[nIndexI]= (byte)(_chKey[nIndexI/8]>>(7-nIndexI%8)&1);

	    for(nIndexI=0;nIndexI<56;nIndexI++)
	    {
		    chTmpKey[nIndexI]=ch64BitKey[PC1[nIndexI]];
	    }

	    for (nIndexI=0;nIndexI<16;nIndexI++)
	    {
		    for (nIndexJ=0;nIndexJ<28;nIndexJ++)
		    {
			    chC[nIndexJ]=chTmpKey[nIndexJ];chD[nIndexJ]=chTmpKey[nIndexJ+28];
		    }
		    //LeftShift
		    for (nIndexJ=0;nIndexJ<28;nIndexJ++)
		    {
			    chTmpKey[nIndexJ]=chC[(nIndexJ+LS[nIndexI])%28];chTmpKey[nIndexJ+28]=chD[(nIndexJ+LS[nIndexI])%28];
		    }
		    //PC2
		    for (nIndexJ=0;nIndexJ<48;nIndexJ++) _chRoundKey[nIndexI][nIndexJ]=chTmpKey[PC2[nIndexJ]];
	    }
    }
    
    void roundFunction(byte []_chCode,int nRoundKeyIndex, byte [][]_chRoundKey)
    {
	    byte []chTmpCode = new byte[48];
		byte[]chBitIn = new byte[6];
	    int nIndexI,nIndexJ;
	    int nRow,nCol,nValSBox;
	    for (nIndexI=0;nIndexI<48;nIndexI++)
		    chTmpCode[nIndexI]=(byte)(_chCode[EP[nIndexI]]^_chRoundKey[nRoundKeyIndex][nIndexI]);
	    for(nIndexI=0;nIndexI<8;nIndexI++)
	    {
		    for (nIndexJ=0;nIndexJ<6;nIndexJ++)chBitIn[nIndexJ]=chTmpCode[nIndexI*6+nIndexJ];
		    for (nIndexJ=nRow=0;nIndexJ<2;nIndexJ++)nRow+=chBitIn[nIndexJ*5]<<(1-nIndexJ);
		    for (nIndexJ=nCol=0;nIndexJ<4;nIndexJ++)nCol+=chBitIn[nIndexJ+1]<<(3-nIndexJ);
		    nValSBox=SBOX[nIndexI][nRow][nCol];
		    for (nIndexJ=0;nIndexJ<4;nIndexJ++)chTmpCode[nIndexI*4+nIndexJ]=(byte)(nValSBox>>(3-nIndexJ)&1);
	    }
	    for (nIndexI=0;nIndexI<32;nIndexI++) _chCode[nIndexI]=chTmpCode[P[nIndexI]];
    }

    void Swap(byte []chLeft,byte []chRight)
    {
	    int nIndexI;
	    byte []chTmp = new byte[32];
	    for (nIndexI=0;nIndexI<32;nIndexI++)
	    {
		    chTmp[nIndexI]=chLeft[nIndexI];
		    chLeft[nIndexI]=chRight[nIndexI];
		    chRight[nIndexI]=chTmp[nIndexI];
	    }
    }


    string enByteArrToHexStr(byte[] _nEn, string _strHex, int _len)
    {
        string strHex = "";
        for (int i = 0; i < _len; i++)
        {

            strHex += intToHexA((int)_nEn[i], 5);
        }
        return strHex;
    }

    void hexStrToByteArr(ref byte []_nEn, string _strHex, int _len)
    {
	    for (int i = 0; i < _len; i+=2)
	    {
		    _nEn[i / 2] = (byte)(hexAToInt(_strHex[i]) << 4);
		    _nEn[i / 2] += (byte)(hexAToInt(_strHex[i + 1]));
	    }
    }

    int hexAToInt(char _hexA)
    {
	    switch(_hexA)
	    {
	    case '0':
		    return 0;
	    case '1':
		    return 1;
	    case '2':
		    return 2;
	    case '3':
		    return 3;
	    case '4':
		    return 4;
	    case '5':
		    return 5;
	    case '6':
		    return 6;
	    case '7':
		    return 7;
	    case '8':
		    return 8;
	    case '9':
		    return 9;
	    case 'A':
		    return 10;
	    case 'B':
		    return 11;
	    case 'C':
		    return 12;
	    case 'D':
		    return 13;
	    case 'E':
		    return 14;
	    case 'F':
		    return 15;
	    }
	    return 0;
    }
    
    string intToHexA( int _n, int _len)
    {
		string result = "";
		int nHi = _n >> 4;
		int nLo = _n & 0xF;
		switch (nHi)
		{
			case 0:
				result = "0";
				break;
			case 1:
				result = "1";
				break;
			case 2:
				result = "2";
				break;
			case 3:
				result = "3";
				break;
			case 4:
				result = "4";
				break;
			case 5:
				result = "5";
				break;
			case 6:
				result = "6";
				break;
			case 7:
				result = "7";
				break;
			case 8:
				result = "8";
				break;
			case 9:
				result = "9";
				break;
			case 10:
				result = "A";
				break;
			case 11:
				result = "B";
				break;
			case 12:
				result = "C";
				break;
			case 13:
				result = "D";
				break;
			case 14:
				result = "E";
				break;
			case 15:
				result = "F";
				break;
		}
		switch (nLo)
		{
			case 0:
				result += "0";
				break;
			case 1:
				result += "1";
				break;
			case 2:
				result += "2";
				break;
			case 3:
				result += "3";
				break;
			case 4:
				result += "4";
				break;
			case 5:
				result += "5";
				break;
			case 6:
				result += "6";
				break;
			case 7:
				result += "7";
				break;
			case 8:
				result += "8";
				break;
			case 9:
				result += "9";
				break;
			case 10:
				result += "A";
				break;
			case 11:
				result += "B";
				break;
			case 12:
				result += "C";
				break;
			case 13:
				result += "D";
				break;
			case 14:
				result += "E";
				break;
			case 15:
				result += "F";
				break;
		}
		return result;
    }

     void textToEnByteArr(string _strText, byte[] strKey, byte[] nEn)
     {
         //set Key
         byte[] chKey = new byte[9];
         int nStrKey = strKey.Length;
         if (nStrKey < 8)
         {
             for (int i = 0; i < 8; i++)
             {
                 if (i < 8 - nStrKey)
                     chKey[i] = 0;
                 else
                     chKey[i] = strKey[i - nStrKey];
             }
         }
         else
             chKey = strKey;

         byte[][] _chRoundKey = new byte[16][]{new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],
 												new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48]};

         keySchedule(chKey, _chRoundKey);
         int nLength;
         int nPlainTextLen = _strText.Length;
         if (nPlainTextLen % 8 == 0) nLength = nPlainTextLen;
         else nLength = (nPlainTextLen / 8 + 1) * 8;
         string strText = _strText;
         for (int k = nPlainTextLen; k < nLength; k++)
         {
             strText += (char)(0);
         }
         char[] chPlainText = strText.ToCharArray();
         int nIndexJ, nIndexI, nIndexK;
         byte[] IV = new byte[8];
         byte[] IV1 = new byte[8];
         IV = chKey;
         for (nIndexI = 0; nIndexI < MAX_TEXT_LEN / 8; nIndexI++)
         {
             byte[] chBuffer = new byte[32];
             byte[] ch64Bit = new byte[64];
             byte[] chLeft64Bit = new byte[32];
             byte[] chRight64Bit = new byte[32];
             byte[] chTmp64Bit = new byte[64];
             if (nIndexI == 0)
             {
                 for (int i = 0; i < 8; i++)
                 {
                     chPlainText[i] ^= (char)IV[i];
                 }
             }
             else
             {
                 for (int i = 0; i < 8; i++)
                 {
                     chPlainText[nIndexI * 8 + i] ^= (char)IV1[i];
                 }
             }
             for (nIndexJ = 0; nIndexJ < 64; nIndexJ++)
             {
                 ch64Bit[nIndexJ] = (byte)(chPlainText[nIndexI * 8 + nIndexJ / 8] >> (7 - nIndexJ % 8) & 1);
             }
             for (nIndexJ = 0; nIndexJ < 64; nIndexJ++) chTmp64Bit[nIndexJ] = ch64Bit[IP[nIndexJ]];
             for (nIndexJ = 0; nIndexJ < 32; nIndexJ++)
             {
                 chLeft64Bit[nIndexJ] = ch64Bit[nIndexJ];
                 chRight64Bit[nIndexJ] = ch64Bit[nIndexJ + 32];
             }
             for (nIndexJ = 0; nIndexJ < 16; nIndexJ++)
             {
                 for (nIndexK = 0; nIndexK < 32; nIndexK++) chBuffer[nIndexK] = chRight64Bit[nIndexK];
                 roundFunction(chBuffer, nIndexJ, _chRoundKey);
                 for (nIndexK = 0; nIndexK < 32; nIndexK++) chLeft64Bit[nIndexK] ^= chBuffer[nIndexK];
                 Swap(chLeft64Bit, chRight64Bit);
             }
             Swap(chLeft64Bit, chRight64Bit);
             for (nIndexJ = 0; nIndexJ < 32; nIndexJ++)
             {
                 chTmp64Bit[nIndexJ] = chLeft64Bit[nIndexJ];
                 chTmp64Bit[nIndexJ + 32] = chRight64Bit[nIndexJ];
             }
             for (nIndexJ = 0; nIndexJ < 64; nIndexJ++)
             {
                 nEn[nIndexI * 8 + nIndexJ / 8] += (byte)(chTmp64Bit[nIndexJ] << (7 - nIndexJ % 8));
             }
             for (int j = 0; j < 8; j++)
             {
                 IV1[j] = nEn[nIndexI * 8 + j];
             }

         }

     }

    void byteArrToDeText(ref char []_strText, byte []_strKey,byte []_nEn, int _len)
    {
	    byte []chEnCode = new byte[MAX_TEXT_LEN + 1];
		chEnCode = _nEn;
	    //set Key
	    byte []chKey = new byte[9];
	    int nStrKey = _strKey.Length;
	    if (nStrKey < 8)
	    {
		    for (int i = 0; i < 8; i++)
		    {
				if(i < 8 - nStrKey)
				    chKey[i] = 0;
				else
					chKey[i] = _strKey[i - nStrKey];
		    }
	    }
	    else
		    chKey = _strKey;
	    byte [][]_chRoundKey = new byte[16][]{new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],
												new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48],new byte[48]};

	    keySchedule(chKey, _chRoundKey);

	    int nIndexJ,nIndexI,nIndexK;
	    byte []IV = new byte[9];
	    byte []IV1 = new byte[9];
	    byte []IV2 = new byte[9];
		IV = chKey;
        for (nIndexI = 0; nIndexI < MAX_TEXT_LEN / 8; nIndexI++)
	    {
		    byte[] chBuffer = new byte[32],ch64Bit = new byte[64],chLeft64Bit = new byte[32],chRight64Bit = new byte[32],chTmp64Bit =new byte[64];
		    {
			    for (int j = 0; j < 8;j++)
			    {
				    IV1[j] = chEnCode[nIndexI*8+j];
			    }
		    }

		    for (nIndexJ=0;nIndexJ<64;nIndexJ++) 
		    {
			    ch64Bit[nIndexJ]=(byte)(chEnCode[nIndexI*8+nIndexJ/8]>>(7-nIndexJ%8)&1);
		    }
		    for (nIndexJ=0;nIndexJ<64;nIndexJ++) chTmp64Bit[nIndexJ]=ch64Bit[IP[nIndexJ]];
		    for (nIndexJ=0;nIndexJ<32;nIndexJ++)
		    {
			    chLeft64Bit[nIndexJ]=ch64Bit[nIndexJ];
			    chRight64Bit[nIndexJ]=ch64Bit[nIndexJ+32];
		    }
		    for (nIndexJ=0;nIndexJ<16;nIndexJ++)
		    {
			    for (nIndexK=0;nIndexK<32;nIndexK++)chBuffer[nIndexK]=chRight64Bit[nIndexK];
			    roundFunction(chBuffer,15 - nIndexJ, _chRoundKey);
			    for (nIndexK=0;nIndexK<32;nIndexK++)chLeft64Bit[nIndexK]^=chBuffer[nIndexK];
			    Swap(chLeft64Bit,chRight64Bit);
		    }
		    Swap(chLeft64Bit,chRight64Bit);
		    for (nIndexJ=0;nIndexJ<32;nIndexJ++)
		    {
			    chTmp64Bit[nIndexJ]=chLeft64Bit[nIndexJ];
			    chTmp64Bit[nIndexJ+32]=chRight64Bit[nIndexJ];
		    }
		    for (nIndexJ=0;nIndexJ<64;nIndexJ++) 
		    {
			    _strText[nIndexI*8+nIndexJ/8]+=(char)(chTmp64Bit[nIndexJ]<<(7-nIndexJ%8));
		    }

		    if (nIndexI  == 0)
		    {
			    for (int i = 0; i < 8; i++)
			    {
				    _strText[nIndexI*8+i] ^= (char)IV[i];
			    }
		    }
		    else
		    {
			    for (int i = 0; i < 8; i++)
			    {
				    _strText[nIndexI*8+i] ^= (char)IV2[i];
			    }

		    }
		    for (int j = 0; j < 8;j++)
		    {
			    IV2[j] = IV1[j];
		    }
	    }
    }
}
