﻿/*
MIT License

Copyright(c) 2020 Evgeny Pereguda

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files(the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions :

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using CaptureManagerToCSharpProxy;
using CaptureManagerToCSharpProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WPFMediaFoundationPlayer
{
    class CaptureManagerVideoRendererMultiSinkFactory
    {

        private CaptureManagerVideoRendererMultiSinkFactory()
        {
            LibraryIndex = 0;
        }

        private bool uploadCaptureManagerToCSharpProxy(
            out ICaptureManagerEVRMultiSinkFactory aICaptureManagerEVRMultiSinkFactory)
        {
            bool lresult = false;

            do
            {
                aICaptureManagerEVRMultiSinkFactory = null;

                CaptureManager mCaptureManager = null;

                try
                {
                    mCaptureManager = new CaptureManager("CaptureManager.dll");
                }
                catch (Exception)
                {
                    try
                    {
                        mCaptureManager = new CaptureManager();
                    }
                    catch (Exception)
                    {

                    }
                }

                if (mCaptureManager == null)
                    break;

                string lXMLSinkString = "";

                mCaptureManager.getCollectionOfSinks(ref lXMLSinkString);

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(lXMLSinkString);

                var lSinkNode = doc.SelectSingleNode("SinkFactories/SinkFactory[@GUID='{A2224D8D-C3C1-4593-8AC9-C0FCF318FF05}']");

                if (lSinkNode == null)
                    break;

                var lMaxPortCountAttributeNode = lSinkNode.SelectSingleNode("Value.ValueParts/ValuePart/@MaxPortCount");

                if (lMaxPortCountAttributeNode == null)
                    break;

                uint lmaxPorts = 0;

                if (!uint.TryParse(lMaxPortCountAttributeNode.Value, out lmaxPorts))
                    break;

                if (lmaxPorts == 0)
                    break;

//                "<SinkFactory Name="CaptureManagerVRMultiSinkFactory" GUID="{A2224D8D-C3C1-4593-8AC9-C0FCF318FF05}" Title="CaptureManager Video Renderer multi sink factory">
//- <Value.ValueParts>
//  <ValuePart Title="Container format" Value="Default" MIME="" Description="Default EVR implementation" MaxPortCount="8" GUID="{E926E7A7-7DD0-4B15-88D7-413704AF865F}" /> 
//  </Value.ValueParts>
//  </SinkFactory>
//"


                ISinkControl lISinkControl = mCaptureManager.createSinkControl();

                if (lISinkControl == null)
                    break;

                IEVRMultiSinkFactory lIEVRMultiSinkFactory = null;

                lISinkControl.createCompatibleEVRMultiSinkFactory(Guid.Empty, out lIEVRMultiSinkFactory);

                if (lIEVRMultiSinkFactory == null)
                    break;

                IEVRStreamControl lIEVRStreamControl = mCaptureManager.createEVRStreamControl();

                if (lIEVRStreamControl == null)
                    break;

                aICaptureManagerEVRMultiSinkFactory = new CaptureManagerEVRMultiSinkFactory(
                    lIEVRMultiSinkFactory,
                    lmaxPorts,
                    lIEVRStreamControl);

                lresult = true;
                                
            }
            while (false);

            return lresult;
        }

        private bool uploadCaptureManagerVideoRendererToCSharpProxy(
            out ICaptureManagerEVRMultiSinkFactory aICaptureManagerEVRMultiSinkFactory)
        {
            aICaptureManagerEVRMultiSinkFactory = null;

            bool lresult = false;

            do
            {
                aICaptureManagerEVRMultiSinkFactory = new CaptureManagerEVRMultiSinkFactory(
                    CMVRMultiSinkFactoryLoader.getInstance().mIEVRMultiSinkFactory,
                    CMVRMultiSinkFactoryLoader.getInstance().MaxPorts,
                    CMVRMultiSinkFactoryLoader.getInstance().mIEVRStreamControl);

                lresult = true;
            }
            while (false);

            return lresult;
        }

        private static CaptureManagerVideoRendererMultiSinkFactory mInstance = null;

        public static CaptureManagerVideoRendererMultiSinkFactory getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new CaptureManagerVideoRendererMultiSinkFactory();
            }

            return mInstance;
        }

        public ICaptureManagerEVRMultiSinkFactory getICaptureManagerEVRMultiSinkFactory()
        {

            ICaptureManagerEVRMultiSinkFactory mICaptureManagerEVRMultiSinkFactory = null;


            if(LibraryIndex == 0)
            {
                uploadCaptureManagerToCSharpProxy(out mICaptureManagerEVRMultiSinkFactory);
            }
            else if (LibraryIndex == 1)
            {
                uploadCaptureManagerVideoRendererToCSharpProxy(out mICaptureManagerEVRMultiSinkFactory);
            }


            return mICaptureManagerEVRMultiSinkFactory;
        }

        public int LibraryIndex { get; set; }
    }
}
