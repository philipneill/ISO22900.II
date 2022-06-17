﻿#region License

// /*
// MIT License
// 
// Copyright (c) 2022 Joerg Frank
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// */

#endregion

using ISO22900.II.UnSafeCStructs;

namespace ISO22900.II
{
    internal class VisitorPduComPrimitiveControlDataMemorySizeUnsafe : IVisitorPduComPrimitiveControlData
    {
        internal int MemorySize { get; set; }

        #region UsedInsidePduStartComPrimitive

        public unsafe void VisitConcretePduCopCtrlData(PduCopCtrlData copCtrlData)
        {
            MemorySize += sizeof(PDU_COP_CTRL_DATA);
            MemorySize += copCtrlData.TxFlag.FlagData.Length * sizeof(byte);
            for (uint index = 0; index < copCtrlData.PduExpectedResponseDatas.Length; index++)
            {
                copCtrlData.PduExpectedResponseDatas[index].Accept(this);
            }
        }

        public unsafe void VisitConcretePduExpectedResponseData(PduExpectedResponseData expectedResponseData)
        {
            MemorySize += sizeof(PDU_EXP_RESP_DATA);
            expectedResponseData.MaskAndPatternPair.Accept(this);
            expectedResponseData.UniqueRespIds.Accept(this);
        }

        public void VisitConcretePduMaskAndPatternBytes(MaskAndPatternBytes maskAndPatternBytes)
        {
            MemorySize += (maskAndPatternBytes.MaskDataArray.Length + maskAndPatternBytes.PatternDataArray.Length) * sizeof(byte);
        }

        public void VisitConcretePduUniqueRespIds(UniqueRespIds uniqueRespIds)
        {
            MemorySize += (int) uniqueRespIds.NumberOfUniqueRespIds * sizeof(uint);
        }
        
        #endregion

    }
}