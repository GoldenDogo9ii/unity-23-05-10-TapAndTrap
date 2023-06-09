using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigInt {
	//ToDo: change to max size of number for presentation
    public static int MAX_DEC_LEN = 45;
		
	//ToDo : change size of INT64 that use 
	//for Linux and 64bit
    private static int BLOCK_DEC_LEN = 4;
	private static int BLOCK_BIT_LEN = 16;

	//size of int64 array
	private static int INT64_ARR_SIZE = MAX_DEC_LEN % BLOCK_DEC_LEN == 0 ? MAX_DEC_LEN / BLOCK_DEC_LEN * 2 : (MAX_DEC_LEN / BLOCK_DEC_LEN + 1) * 2;

//	private static int INT64_REAL_ARR_SIZE = MAX_DEC_LEN % BLOCK_DEC_LEN == 0 ? MAX_DEC_LEN / BLOCK_DEC_LEN : (MAX_DEC_LEN / BLOCK_DEC_LEN + 1);
	//Max decimal length of BigInt
//	private static int MAX_BIGINT_DEC_LEN = INT64_ARR_SIZE * BLOCK_DEC_LEN;
	//max bit length of BigInt
//	private static int MAX_BIGINT_BIN_LEN = INT64_ARR_SIZE * BLOCK_BIT_LEN;

	//variable to present for sign
	public bool						mIsNegative;
	//Int64 Array to save data
	public long[] mBigInt = new long[INT64_ARR_SIZE];

    private static long mMaxNumBlock;
	//----------------------------
	//	BigInt : DataType to compute long int
	//	Max Dec Len :80
	//	operators:+,-,*,/,%(mod),>>,<<,
	//	copyright 2017
	//	write by Han Gum Nam
	//----------------------------
	public BigInt()
	{
		for (int n =0;n<INT64_ARR_SIZE;n++)
		{
			mBigInt[n] = 0;
		}
		mIsNegative = false;

		long nOne = 1;
		mMaxNumBlock = nOne << BLOCK_BIT_LEN;
	}
	public BigInt(BigInt _init)
	{
		for(int n = 0; n < INT64_ARR_SIZE; n++)
		{
			mBigInt[n] = _init.mBigInt[n];
		}
		mIsNegative = _init.mIsNegative;

		long nOne = 1;
		mMaxNumBlock = nOne << BLOCK_BIT_LEN;
	}
	public BigInt(long _n, bool _isNegative)
	{
		long nOne = 1;
		mMaxNumBlock = nOne << BLOCK_BIT_LEN;
		InitFromInt(_n, _isNegative);
	}
	//-----------------------------
	//	function name :Constructor
	//	parameter:_strBigInt	;char array to convert BigInt
	//								;max length :80
	//				
	//-----------------------------
	public BigInt(string _strBigInt, bool _isNegativ)
	{
		long nOne = 1;
		mMaxNumBlock = nOne << BLOCK_BIT_LEN;

		InitFromStr(_strBigInt, _isNegativ);
	}


	//-----------------------------
	//	function name	:one
	//	remark			:create one in BigInt
	//	parameter			:_strBigInt	;char array to convert BigInt
	//										;max length :80
	//				
	//-----------------------------
	public static BigInt one()
	{
		BigInt result = new BigInt(1,false);
		return result;
	}
	//-----------------------------
	//	function name	:InitFromStr
	//	remark			:Initialize BigInt from decimal char array
	//	parameter			:_strBigInt	;char array to convert BigInt
	//										;max length :80
	//	result				:
	//-----------------------------
	public void InitFromStr(string _strBigInt, bool _isNegativ)
	{
		int nStrTextLen = _strBigInt.Length;
		if (nStrTextLen > MAX_DEC_LEN)
			return;

		//reset BigInt
		mIsNegative = _isNegativ;
		for (int n = 0;n < INT64_ARR_SIZE; n++)
			mBigInt[n] = 0;


		int nBit4BlockSize = BLOCK_BIT_LEN  / 4; 
		int nStrIdx = 0;

		//make BCD from decimal string.
		long nTmp = 0;
		for (nStrIdx = 0; nStrIdx < nStrTextLen; nStrIdx++)
		{
			nTmp = (long)(_strBigInt[nStrTextLen - nStrIdx - 1] - 48);
			mBigInt[nStrIdx / nBit4BlockSize] = mBigInt[nStrIdx / nBit4BlockSize] | (nTmp << (nStrIdx % nBit4BlockSize * 4));
		} 

		//Convert BCD to Binary;
		long nHighBlock = 0;
		long nLowBlock = 0;
		long nHighMask = 0, nLowMask = 0;
		long nMyIntMask =mMaxNumBlock - 1;

		int nHighShiftCnt = 0;
		int nLowShiftCnt = 0;
		int nHighIdx = 0;
		int nLowIdx = 0;

		long nTmpBlock = 0;

		for (int n = 1;n < nStrTextLen ; n++)
		{
			for (nStrIdx = nStrTextLen -1;nStrIdx>=n;nStrIdx--)
			{
				nHighShiftCnt = nStrIdx % nBit4BlockSize * 4;
				nLowShiftCnt = (nStrIdx - 1) % nBit4BlockSize * 4;

				nHighIdx = nStrIdx / nBit4BlockSize;
				nLowIdx = (nStrIdx - 1) / nBit4BlockSize;

				nHighBlock = (mBigInt[nHighIdx] >> nHighShiftCnt) & 0xF;
				nLowBlock = (mBigInt[nLowIdx] >> nLowShiftCnt) & 0xF; 

				//nHighMask = mBigInt[nHighIdx] - (nHighBlock << (nStrIdx % nBit4BlockSize * 4));
				//nLowMask = mBigInt[nLowIdx] - (nLowBlock << nLowShiftCnt);

				nHighMask = nMyIntMask - (nHighBlock << nHighShiftCnt);
				nLowMask = nMyIntMask - (nLowBlock << nLowShiftCnt);


				mBigInt[nHighIdx] = mBigInt[nHighIdx] & nHighMask;
				mBigInt[nLowIdx] = mBigInt[nLowIdx] & nLowMask;

				nTmpBlock = nHighBlock * 10 + nLowBlock;
				nHighBlock = nTmpBlock / 16;
				nLowBlock = nTmpBlock % 16;

				mBigInt[nHighIdx] = mBigInt[nHighIdx]  | ((nHighBlock << nHighShiftCnt) & nMyIntMask);
				mBigInt[nLowIdx] = mBigInt[nLowIdx] | ((nLowBlock << nLowShiftCnt) & nMyIntMask);

			}
		}
	}

	//-----------------------------
	//	function name	:InitFromBinaryStr
	//	remark			:Initialize BigInt from binary char array
	//	parameter		:_strBigInt	;char array to convert BigInt
	//								;max length :280
	//	result				:
	//-----------------------------
	public void InitFromBinaryStr(string _binStr)
	{
		BigInt biResult = new BigInt();
		long nOne = 1;
		int nstrlen = _binStr.Length;
		for (int k =0;k < nstrlen;k++)
		{
			biResult = biResult << 1;
			if (_binStr[k] == 49)
			{
				biResult.mBigInt[0] = biResult.mBigInt[0] | nOne;
			}
		}

		initFromMyIntArr(biResult.mBigInt, biResult.mIsNegative);
	}
	public void	InitFromInt( long _n, bool _isNegative)
	{
		mIsNegative = _isNegative;
		long nOne = 1;
		mBigInt[0] = _n % (nOne << BLOCK_BIT_LEN);
		mBigInt[1] = _n / (nOne << BLOCK_BIT_LEN);
		for (int n = 2; n < INT64_ARR_SIZE; n++)
		{
			mBigInt[n] = 0;
		}
	}
	public void	InitToZero()
	{
		mIsNegative = false;
		for (int n = 0; n < INT64_ARR_SIZE; n++)
		{
			mBigInt[n] = 0;
		}
	}
	//-----------------------------
	//	function name	:InitFromBinaryStr
	//	remark			:Convert BigInt to decimal char array
	//	parameter			:
	//	result				:decimal char array
	//-----------------------------
	public string ToStr()
	{
		string sText = "";
		BigInt tmpThis = new BigInt(this);
		int nBit4BlockSize = BLOCK_BIT_LEN  / 4;
		int nStrIdx = 0; 

		//Convert Binary to BCD;
		long nHighBlock = 0;
		long nLowBlock = 0;
		long nHighMask = 0, nLowMask = 0;
		long nMyIntMask =mMaxNumBlock - 1;

		int nHighShiftCnt = 0;
		int nLowShiftCnt = 0;
		int nHighIdx = 0;
		int nLowIdx = 0;

		long nTmpBlock = 0;
		int nBit4BlockAllSize = INT64_ARR_SIZE * nBit4BlockSize;
		for (int n = 1; n < nBit4BlockAllSize; n++)
		{
			for (nStrIdx = nBit4BlockAllSize -1; nStrIdx>=n; nStrIdx--)
			{
				nHighShiftCnt = nStrIdx % nBit4BlockSize * 4;
				nLowShiftCnt = (nStrIdx - 1) % nBit4BlockSize * 4;

				nHighIdx = nStrIdx / nBit4BlockSize;
				nLowIdx = (nStrIdx - 1) / nBit4BlockSize;

				nHighBlock = (tmpThis.mBigInt[nHighIdx] >> nHighShiftCnt) & 0xF;
				nLowBlock = (tmpThis.mBigInt[nLowIdx] >> nLowShiftCnt) & 0xF; 

 				nHighMask = tmpThis.mBigInt[nHighIdx] - (nHighBlock << nHighShiftCnt);
 				nLowMask = tmpThis.mBigInt[nLowIdx] - (nLowBlock  << nLowShiftCnt);

				nHighMask = nMyIntMask - (nHighBlock << nHighShiftCnt);
				nLowMask = nMyIntMask - (nLowBlock << nLowShiftCnt);

				tmpThis.mBigInt[nHighIdx] = tmpThis.mBigInt[nHighIdx] & nHighMask;
				tmpThis.mBigInt[nLowIdx] = tmpThis.mBigInt[nLowIdx] & nLowMask;

				nTmpBlock = nHighBlock * 16 + nLowBlock;
				nHighBlock = nTmpBlock / 10;
				nLowBlock = nTmpBlock % 10;

				tmpThis.mBigInt[nHighIdx] = tmpThis.mBigInt[nHighIdx]  | ((nHighBlock << nHighShiftCnt) & nMyIntMask);
				tmpThis.mBigInt[nLowIdx] = tmpThis.mBigInt[nLowIdx] | ((nLowBlock << nLowShiftCnt) & nMyIntMask);

			}
		}

		//convert BCD to decimal string.
		int nBCDLen = tmpThis.getShiftCnt() / 4;
		for (nStrIdx = nBCDLen - 1; nStrIdx >= 0; nStrIdx--)
		{
			sText += (tmpThis.mBigInt[nStrIdx / nBit4BlockSize] >> (nStrIdx % nBit4BlockSize * 4) & 0xF);
		}
		return sText;
	}
	//-----------------------------
	//	function name	:InitFromBinaryStr
	//	remark			:Convert BigInt to binary char array
	//	parameter			:
	//	result				:binary char array
	//-----------------------------
	public string ToBinaryStr()
	{
		string result = "";
		BigInt tmpHexThis = new BigInt(this);
		int nMyIntBlockCnt = tmpHexThis.getShiftCnt() % BLOCK_BIT_LEN == 0? tmpHexThis.getShiftCnt() / BLOCK_BIT_LEN : tmpHexThis.getShiftCnt() / BLOCK_BIT_LEN + 1;
		long nTmpLast =  tmpHexThis.mBigInt[nMyIntBlockCnt - 1];
		int n;
		int nLastShft = 0;
		for (n = BLOCK_BIT_LEN - 1; n >=0 ;n--)
		{
			if ((nTmpLast >> n)  ==1)
			{
				nLastShft = n;
				break;
			}
		}
		long nOne = 1;
		long nMaskLast = nOne << nLastShft;
		int nShiftCnt =  (nMyIntBlockCnt - 1) * BLOCK_BIT_LEN +  n + 1;
		for (n = 0;n < nShiftCnt; n++)
		{
			if ((nTmpLast >> nLastShft)  ==1)
			{
				result += "1";
			}
			else
			{
				result += "0";
			}
			tmpHexThis = tmpHexThis << 1;
			nTmpLast =  tmpHexThis.mBigInt[nMyIntBlockCnt - 1] & nMaskLast;
		
		}

		return result;

	}
	void initFromMyIntArr(long[] _nmyInt,bool _isNegative)
	{
		for (int n=0; n < INT64_ARR_SIZE; n++)
		{
			mBigInt[n] = _nmyInt[n];
		}
		mIsNegative = _isNegative;
	}


	public static BigInt operator + (BigInt _leftOper, BigInt _rigOper)
	{
		BigInt result = new BigInt();
		BigInt augend = new BigInt(_leftOper);
		BigInt addend = new BigInt(_rigOper);
		if (_leftOper.mIsNegative && !_rigOper.mIsNegative)
		{
			augend.mIsNegative = false;
			result = addend - augend;
			return result;
		}
		else if (!_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			addend.mIsNegative = false;
			result = augend - addend;
			return result;
		}
		if (_leftOper.mIsNegative && _rigOper.mIsNegative)
			result.mIsNegative = true;
		long nAdd = 0;
		long nSum = 0;
		for (int n = 0; n < INT64_ARR_SIZE; n++)
		{
			augend.mBigInt[n] += nAdd;
			nSum = (augend.mBigInt[n] + addend.mBigInt[n]) % mMaxNumBlock;
			nAdd = (augend.mBigInt[n] + addend.mBigInt[n]) / mMaxNumBlock;
			result.mBigInt[n] = nSum;
		}
		return result;
	}
	public static BigInt	 operator	+(BigInt _biLeft, long _nRight)
	{
		BigInt result = new BigInt(_biLeft);
		long nOne = 1;
		long nMaxNumBlock = nOne << BLOCK_BIT_LEN;
		long nAdd = 0;
		for (int n = 0; n < INT64_ARR_SIZE; n++)
		{
			if (n == 0)
			{
				result.mBigInt[n] = (result.mBigInt[n] + _nRight % nMaxNumBlock) % nMaxNumBlock;
			}
			else if (n == 1)
			{
				result.mBigInt[n] = (result.mBigInt[n] + _nRight / nMaxNumBlock + nAdd) % nMaxNumBlock;
			}
			else
			{
				if (nAdd == 0) 
					break;
				result.mBigInt[n] = (result.mBigInt[n]  + nAdd) % nMaxNumBlock;
			}
			nAdd = result.mBigInt[n] / nMaxNumBlock;
		}
		return result;
	}
	public static BigInt operator -(BigInt _leftOper, BigInt _rigOper)
	{
		BigInt result = new BigInt();
		BigInt subtrahend = new BigInt(_leftOper);
		BigInt minuend = new BigInt(_rigOper);
		if (subtrahend.mIsNegative && !minuend.mIsNegative)
		{
			subtrahend.mIsNegative = false;
		
			result = minuend + subtrahend;
			result.mIsNegative = true;
			return result;
		}
		else if (!subtrahend.mIsNegative && minuend.mIsNegative)
		{
			minuend.mIsNegative = false;
			result = subtrahend + minuend;
			return result;
		}
		if(subtrahend.mIsNegative && minuend.mIsNegative)
		{
			BigInt temp2 = new BigInt(minuend);
			subtrahend.mIsNegative = false;
			minuend.mIsNegative = false;
			minuend = subtrahend;
			subtrahend = temp2;
		}
		if (!(subtrahend >= minuend))
		{
			result.mIsNegative = true;
			BigInt tmp = new BigInt(minuend);
			minuend = subtrahend;
			subtrahend = tmp;
		}
		long nSub = 0;
		long nReminder = 0;
		long tmpSubstrahend = 0;
		for (int n = 0; n < INT64_ARR_SIZE; n++)
		{
			tmpSubstrahend = subtrahend.mBigInt[n];
			tmpSubstrahend -= nSub;
			nSub = 0;
			if (tmpSubstrahend >= (long)minuend.mBigInt[n])
			{
				nReminder =tmpSubstrahend - minuend.mBigInt[n];
			}
			else
			{
				if( n != (INT64_ARR_SIZE - 1))
				{
					nReminder = tmpSubstrahend - minuend.mBigInt[n]+mMaxNumBlock;
					nSub = 1;
				}
				else
				{
					nReminder =tmpSubstrahend - minuend.mBigInt[n];
					result.mIsNegative = true;
				}
			}
			result.mBigInt[n] = nReminder;
		}
		return result;
	}
	public static BigInt operator * (BigInt _leftOper, BigInt _rigOper)
	{
		BigInt result = new BigInt();
		BigInt multiplicand = new BigInt(_leftOper);
		BigInt multiplier = new BigInt(_rigOper);

		if (_leftOper.mIsNegative && !_rigOper.mIsNegative)
		{
			result.mIsNegative = true;
		}
		else if (!_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			result.mIsNegative = true;
		}
		if (_leftOper >= _rigOper)
		{
			multiplicand.initFromMyIntArr(_leftOper.mBigInt,_leftOper.mIsNegative);
			multiplier.initFromMyIntArr(_rigOper.mBigInt, _rigOper.mIsNegative);
		}
		else
		{
			multiplicand.initFromMyIntArr(_rigOper.mBigInt, _rigOper.mIsNegative);
			multiplier.initFromMyIntArr(_leftOper.mBigInt, _leftOper.mIsNegative);
		}
		multiplicand.mIsNegative = false;
		multiplier.mIsNegative = false;

		int nShiftCnt = multiplicand.getShiftCnt();
		int SHIFTCNT = nShiftCnt;
		BigInt biTmpMultiplier = new BigInt(multiplier);
		biTmpMultiplier = biTmpMultiplier << SHIFTCNT;
		while(nShiftCnt > 0)
		{
			if ((multiplicand.mBigInt[0] & 1) == 1)
			{
				multiplicand = multiplicand + biTmpMultiplier;
				multiplicand = multiplicand >> 1;
			}
			else
			{
				multiplicand = multiplicand >> 1;
			}
			--nShiftCnt;
		}
		result.initFromMyIntArr(multiplicand.mBigInt,result.mIsNegative);
		return result;
	}
	public static BigInt operator / (BigInt _leftOper, BigInt _rigOper)
	{
		BigInt dividend = new BigInt(_leftOper);
		BigInt divisor = new BigInt(_rigOper);
		BigInt rest = new BigInt();
		bool bIsNegative = false;
		if (_leftOper.mIsNegative && !_rigOper.mIsNegative)
		{
			bIsNegative				= true;
			dividend.mIsNegative	= false;
		}
		else if (!_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			bIsNegative				= true;
			divisor.mIsNegative			= false;
		}
		BigInt tmpRest = new BigInt();
		int nShiftCnt = dividend.getShiftCnt();
		int nLastIdx = nShiftCnt / BLOCK_BIT_LEN;
		long nOne = 1;
		long nLastMask = (nOne << ( nShiftCnt % BLOCK_BIT_LEN)) - 1;
		long nLast = 0;
		int nMaskLen = nShiftCnt % BLOCK_BIT_LEN;
		while(nShiftCnt > 0)
		{
			// Left Shift
			dividend	= dividend << 1;
			nLast = dividend.mBigInt[nLastIdx] >> nMaskLen;
			dividend.mBigInt[nLastIdx] = dividend.mBigInt[nLastIdx] & nLastMask;
			rest = rest << 1;
			rest = rest | nLast;

			tmpRest = rest;
			rest = rest - divisor;

			if (rest.mIsNegative)
			{
				rest = tmpRest;
			}
			else
			{
				dividend = dividend + 1;
			}
			--nShiftCnt;
		}
		dividend.mIsNegative = bIsNegative;
		return dividend;
	}
	public static BigInt operator	<<(BigInt _leftOper, int _n)
	{
		int multi = _n / BLOCK_BIT_LEN;
		int rest = _n % BLOCK_BIT_LEN;
		int rtShCnt = 0;
		BigInt result = new BigInt (_leftOper);

		long nMask32 =mMaxNumBlock - 1;
		long nShiftTmp = 0;
		long nPrevShiftTmp = 0;
		long nOne = 1;
		long nMask = 0;
		for (int k = 0;k <= multi;k++)
		{
			rtShCnt = (k == 0 ? rest : BLOCK_BIT_LEN);
			if (rtShCnt == 0)
			{
				continue;
			}
			nMask = (nOne << (BLOCK_BIT_LEN - rtShCnt)) - 1;
			for (int nIdxI =0; nIdxI<INT64_ARR_SIZE; nIdxI++)
			{
				nShiftTmp = result.mBigInt[nIdxI] & (nMask32 - nMask);
				nShiftTmp = nShiftTmp >> (BLOCK_BIT_LEN - rtShCnt);
				result.mBigInt[nIdxI] = result.mBigInt[nIdxI] <<  rtShCnt;
				result.mBigInt[nIdxI] = result.mBigInt[nIdxI] | nPrevShiftTmp;
				result.mBigInt[nIdxI] = result.mBigInt[nIdxI] & nMask32;
				nPrevShiftTmp = nShiftTmp;
			}
		}
		return result;
	}
	public static BigInt operator	>>(BigInt _leftOper, int _n)
	{
		int multi = _n / BLOCK_BIT_LEN;
		int rest = _n % BLOCK_BIT_LEN;

		int rtShCnt = 0;
		BigInt result = new BigInt (_leftOper);
		long nMask = 0;
		long nOne = 1;
		long nShiftTmp = 0;
		long nPrevShiftTmp = 0;
		for (int k = 0; k <= multi; k++)
		{
			rtShCnt = k == 0 ? rest : BLOCK_BIT_LEN;
			if (rtShCnt == 0)
			{
				continue;
			}
			nMask = (nOne << rtShCnt)  - 1;
			nShiftTmp = 0;
			nPrevShiftTmp = 0;
			for (int nIdxI =(INT64_ARR_SIZE - 1); nIdxI >=0; nIdxI--)
			{
				nShiftTmp = result.mBigInt[nIdxI] & ( nMask);
				nShiftTmp = nShiftTmp << (BLOCK_BIT_LEN - rtShCnt);
				result.mBigInt[nIdxI] = result.mBigInt[nIdxI] >>  rtShCnt;
				result.mBigInt[nIdxI] = result.mBigInt[nIdxI] | nPrevShiftTmp;
				nPrevShiftTmp = nShiftTmp;
			}
		}
		return result;
	}
	public static BigInt operator	&(BigInt _leftOper, BigInt _rigOper)
	{
		BigInt result = new BigInt();
		for(int idxI = 0; idxI < INT64_ARR_SIZE; idxI++)
		{
			result.mBigInt[idxI] = _leftOper.mBigInt[idxI] & _rigOper.mBigInt[idxI];
		}
		return result;
	}
	public static BigInt operator	&(BigInt _biLeft, long _rigOper)
	{
		BigInt result = new BigInt();
		long nOne = 1;
		long nMaxNumBlock =   nOne << BLOCK_BIT_LEN;
		if (_rigOper > nMaxNumBlock)
		{
			result.mBigInt[0] = _biLeft.mBigInt[0] &(_rigOper % nMaxNumBlock);
			result.mBigInt[0] = _biLeft.mBigInt[1] &(_rigOper / nMaxNumBlock);
		}
		else
		{
			result.mBigInt[0] = _biLeft.mBigInt[0] &_rigOper;
		}
		return result;
	}
	public static BigInt operator	|(BigInt _biLeft, long _rigOper)
	{
		BigInt result = new BigInt(_biLeft);
		long nOne = 1;
		long nMaxNumBlock = nOne << BLOCK_BIT_LEN;
		if (_rigOper > nMaxNumBlock)
		{
			result.mBigInt[0] = result.mBigInt[0] |(_rigOper % nMaxNumBlock);
			result.mBigInt[0] = result.mBigInt[1] |(_rigOper / nMaxNumBlock);
		}
		else
		{
			result.mBigInt[0] = result.mBigInt[0] |_rigOper;
		}
	
		return result;
	}
	 public static BigInt operator	&(long _nLeft,BigInt _biRight)
	{
		BigInt result  = new BigInt();
		long nOne = 1;
		long nMaxNumBlock = nOne << BLOCK_BIT_LEN;
		if (_nLeft > nMaxNumBlock)
		{
			result.mBigInt[0] = _biRight.mBigInt[0] &(_nLeft % nMaxNumBlock);
			result.mBigInt[0] = _biRight.mBigInt[1] &(_nLeft / nMaxNumBlock);
		}
		else
		{
			result.mBigInt[0] = _biRight.mBigInt[0] &_nLeft;
		}
		return result;
	}
	public static BigInt	 operator	|(long _nLeft,BigInt _biRight)
	{
		BigInt result = new BigInt(_biRight);
		long nOne = 1;
		long nMaxNumBlock = nOne << BLOCK_BIT_LEN;
		if (_nLeft > nMaxNumBlock)
		{
			result.mBigInt[0] = result.mBigInt[0] |(_nLeft % nMaxNumBlock);
			result.mBigInt[0] = result.mBigInt[1] |(_nLeft / nMaxNumBlock);
		}
		else
		{
			result.mBigInt[0] = result.mBigInt[0] |_nLeft;
		}

		return result;
	}
	public static BigInt operator	|(BigInt _leftOper,BigInt _rigOper)
	{
		BigInt result = new BigInt();
		for (int idxI =0;idxI<INT64_ARR_SIZE; idxI++)
		{
			result.mBigInt[idxI] = _leftOper.mBigInt[idxI] | _rigOper.mBigInt[idxI];
		}
		return result;
	}
	public static BigInt operator % (BigInt _leftOper,BigInt _rigOper)
	{
		BigInt divisor		= new BigInt(_rigOper);
		BigInt dividend		= new BigInt(_leftOper);
	
		BigInt rest = new BigInt();
		bool bIsNegative = false;
		BigInt tmpRest = new BigInt();
		int nShiftCnt = dividend.getShiftCnt();

		int nLastIdx = nShiftCnt / BLOCK_BIT_LEN;
		long nOne = 1;
		long nLastMask = (nOne << ( nShiftCnt % BLOCK_BIT_LEN)) - 1;
		long nLast = 0;
		int nMaskLen = nShiftCnt % BLOCK_BIT_LEN;

		dividend.mIsNegative = false;
		while(nShiftCnt > 0)
		{
			// Left Shift
			dividend	= dividend << 1;
			nLast = dividend.mBigInt[nLastIdx] >> nMaskLen;
			dividend.mBigInt[nLastIdx] = dividend.mBigInt[nLastIdx] & nLastMask;
			rest = rest << 1;
			rest =  nLast | rest;

			tmpRest = rest;
			rest = rest - divisor;
			if (rest.mIsNegative)
			{
				rest = tmpRest;
			}
			--nShiftCnt;
		}
		rest.mIsNegative = bIsNegative;
		return rest;
	}
	static public bool operator >=(BigInt _leftOper, BigInt _rigOper)
	{
		bool result = false;
		if(_leftOper.mIsNegative && !_rigOper.mIsNegative)
		{
			return false;
		}
		else if(!_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			return true;
		}
		for(int n = INT64_ARR_SIZE - 1; n >= 0; n--)
		{
			if(_leftOper.mBigInt[n] > _rigOper.mBigInt[n])
			{
				result = true;
				break;
			}
			else if(_leftOper.mBigInt[n] < _rigOper.mBigInt[n])
			{
				result = false;
				break;
			}
			else
			{
				if(n == 0)
				{
					result = true;
				}
			}
		}
		if(_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			result = !result;
		}
		return result;
	}
	static public bool operator <=(BigInt _leftOper, BigInt _rigOper)
	{
		bool result = false;
		if(_leftOper.mIsNegative && !_rigOper.mIsNegative)
		{
			return true;
		}
		else if(!_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			return false;
		}
		for(int n = INT64_ARR_SIZE - 1; n >= 0; n--)
		{
			if(_leftOper.mBigInt[n] > _rigOper.mBigInt[n])
			{
				result = false;
				break;
			}
			else if(_leftOper.mBigInt[n] < _rigOper.mBigInt[n])
			{
				result = true;
				break;
			}
			else
			{
				if(n == 0)
				{
					result = true;
				}
			}
		}
		if(_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			result = !result;
		}
		return result;
	}
	static public bool IsEqual(BigInt _leftOper, BigInt _rigOper)
	{
		bool result = false;
		if(_leftOper.mIsNegative && !_rigOper.mIsNegative)
		{
			return false;
		}
		else if(!_leftOper.mIsNegative && _rigOper.mIsNegative)
		{
			return false;
		}
		int n = 0;
		for(n = INT64_ARR_SIZE - 1; n >= 0; n--)
		{
			if(_leftOper.mBigInt[n] != _rigOper.mBigInt[n])
			{
				result = false;
				break;
			}
		}
		if(n == -1)
			result = true;
		return result;
	}
	static public bool IsUnEqual (BigInt _leftOper, BigInt _rigOper)
	{
		bool result = false;
		result = !IsEqual(_leftOper, _rigOper);
		return result;
	}
	int getShiftCnt()
	{
		int result = 0;
		int nBockIdx = 0;
		int nBitIdx = 0;
		for (nBockIdx = INT64_ARR_SIZE - 1;nBockIdx >= 0; nBockIdx--)
		{
			if (mBigInt[nBockIdx] != 0)
			{
				break;
			}
		}
		nBockIdx = nBockIdx<0 ? 0 : nBockIdx;

		for (nBitIdx = 1; nBitIdx <= BLOCK_BIT_LEN/4; nBitIdx++)
		{
			long nTmpNum = mBigInt[nBockIdx] << nBitIdx * 4;
			nTmpNum = nTmpNum >> BLOCK_BIT_LEN;
			if (nTmpNum != 0)
			{
				break;
			}
		}
		result = nBockIdx * BLOCK_BIT_LEN +BLOCK_BIT_LEN  - nBitIdx * 4 +4;
		return result;
	}
	public static long PowMyInt(long _X, long _Y)
	{
		
		uint _N;
		if(_Y >= 0)
			_N = (uint)_Y;
		else
			_N = (uint)(-_Y);
		for(long _Z = 1; ; _X *= _X)
		{
			if((_N & 1) != 0)
				_Z *= _X;
			if((_N >>= 1) == 0)
				return (_Y < 0 ? 1 / _Z : _Z);
		}
	
	}
	public char[] BigIntToCharArr()
	{
		char []result = new char[8];
		BigInt _n = new BigInt(this);

		int nM = BLOCK_BIT_LEN / 4;
		long nHigh = 0;
		long hLow = 0;
		for(int k = 0;k < 8; k++)
		{
			nHigh = ((_n.mBigInt[(nM -  2 * k) /nM] >> ((( nM -  2 * k) % nM) * 4)) & 0xF) << 4;
			hLow = (_n.mBigInt[(nM -  2 * k - 1) /nM] >> ((( nM -  2 * k - 1) % nM) * 4)) & 0xF;
			result[k] =  (char)(nHigh +  hLow);
		}
		return result;

	}
	public static BigInt BiPowMod(BigInt _base, BigInt _ex, BigInt _n)
	{
		string strBinEx = _ex.ToBinaryStr();
		BigInt result = new BigInt(1,false);
		int nStrBinExLen = strBinEx.Length;
		char[] chBinEx = strBinEx.ToCharArray();
		for (int nIdx = 0; nIdx < nStrBinExLen; nIdx++)
		{
			result = (result * result) % _n;
			if (chBinEx[nIdx] == 49)
			{
				result = (result * _base) % _n;
			}
		}
		return result;
	}
	public static string LongToBinStr(long _val)
	{
		string result = "";
		long temp = _val;
		while (temp != 0)
		{
			result = (temp % 2).ToString() + result;
			temp = temp / 2;
		}
		return result;
	}
}