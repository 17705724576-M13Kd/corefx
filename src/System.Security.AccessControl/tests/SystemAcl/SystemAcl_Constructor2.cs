// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Security.AccessControl.Tests
{
    public partial class SystemAcl_Constructor2
    {
       public static IEnumerable<object[]> SystemAcl_Constructor2_TestData()
       {
           yield return new object[] { false, false, 0, 0 };
           yield return new object[] { false, true, 127, 0 };
           yield return new object[] { true, false, 255, 0 };
           yield return new object[] { true, true, 0, 0 };
           yield return new object[] { false, false, 127, 1 };
           yield return new object[] { false, true, 255, 1 };
           yield return new object[] { true, false, 0, 1 };
           yield return new object[] { true, true, 255, 1 };
        }

        [Fact]
        public static void AdditionalTestCases()
        {
            SystemAcl systemAcl = null;
            bool isContainer = false;
            bool isDS = false;
            byte revision = 0;
            int capacity = 0;

            //case 1, capacity = -1
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                capacity = -1;
                systemAcl = new SystemAcl(isContainer, isDS, revision, capacity);
            });

            //case 2, capacity = Int32.MaxValue/2
            Assert.Throws<OutOfMemoryException>(() =>
            {
                isContainer = true;
                isDS = false;
                revision = 0;
                capacity = Int32.MaxValue / 2;
                TestConstructor(isContainer, isDS, revision, capacity);
            });

            //case 3, capacity = Int32.MaxValue
            Assert.Throws<OutOfMemoryException>(() =>
            {
                isContainer = true;
                isDS = true;
                revision = 255;
                capacity = Int32.MaxValue;
                TestConstructor(isContainer, isDS, revision, capacity);
            });
        }

        [Theory]
        [MemberData(nameof(SystemAcl_Constructor2_TestData))]
        public static void TestConstructor(bool isContainer, bool isDS, byte revision, int capacity)
        {
            bool result = true;
            byte[] sAclBinaryForm = null;
            byte[] rAclBinaryForm = null;

            SystemAcl systemAcl = null;
            RawAcl rawAcl = null;

            systemAcl = new SystemAcl(isContainer, isDS, revision, capacity);
            rawAcl = new RawAcl(revision, capacity);

            if (isContainer == systemAcl.IsContainer &&
                isDS == systemAcl.IsDS &&
                revision == systemAcl.Revision &&
                0 == systemAcl.Count &&
                8 == systemAcl.BinaryLength &&
                true == systemAcl.IsCanonical)
            {
                sAclBinaryForm = new byte[systemAcl.BinaryLength];
                rAclBinaryForm = new byte[rawAcl.BinaryLength];
                systemAcl.GetBinaryForm(sAclBinaryForm, 0);
                rawAcl.GetBinaryForm(rAclBinaryForm, 0);

                if (!Utils.IsBinaryFormEqual(sAclBinaryForm, rAclBinaryForm))
                    result = false;
                //redundant index check
                for (int i = 0; i < systemAcl.Count; i++)
                {
                    if (!Utils.IsAceEqual(systemAcl[i], rawAcl[i]))
                    {
                        result = false;
                        break;
                    }
                }

            }
            else
            {
                result = false;
            }
            Assert.True(result);
        }
    }
}