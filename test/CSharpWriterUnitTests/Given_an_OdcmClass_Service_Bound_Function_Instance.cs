﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Its.Recipes;
using Microsoft.MockService;
using Microsoft.MockService.Extensions.ODataV4;
using Microsoft.OData.ProxyExtensions;
using Vipr.Core.CodeModel;
using Xunit;

namespace CSharpWriterUnitTests
{
    public class Given_an_OdcmClass_Service_Bound_Function_Instance : Given_an_OdcmClass_Service_Bound_Function_Base
    {
        public Given_an_OdcmClass_Service_Bound_Function_Instance()
        {
            IsCollection = false;

            ReturnTypeGenerator = (t) => typeof(Task<>).MakeGenericType(t);

            Init();
        }

        [Fact]
        public void The_Service_parses_the_response()
        {
            Init(m =>
            {
                m.Verbs = OdcmAllowedVerbs.Get;
                m.Parameters.Clear();
            });

            var responseKeyValues = Class.GetSampleKeyArguments().ToArray();
            var response = ConcreteType.Initialize(responseKeyValues);

            using (var mockService = new MockService())
            {
                mockService
                    .OnInvokeMethodRequest("GET",
                        "/" + Method.FullName,
                        null,
                        null)
                    .RespondWithGetEntity(TargetEntity.Class.GetDefaultEntitySetName(), response);

                var service = mockService
                    .CreateContainer(EntityContainerType, Any.TokenGetterFunction());

                var result = service.InvokeMethod<Task>(Method.Name + "Async").GetPropertyValue<EntityBase>("Result");

                result.ValidatePropertyValues(responseKeyValues);
            }
        }
    }
}
