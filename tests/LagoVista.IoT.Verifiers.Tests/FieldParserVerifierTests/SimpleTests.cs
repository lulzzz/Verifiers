﻿using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using LagoVista.IoT.Runtime.Core.Models.PEM;
using LagoVista.IoT.Runtime.Core.Models.Verifiers;
using LagoVista.IoT.Runtime.Core.Module;
using LagoVista.IoT.Verifiers.Models;
using LagoVista.IoT.Verifiers.Repos;
using LagoVista.IoT.Verifiers.Runtime;
using LagoVista.IoT.Verifiers.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Verifiers.Tests.FieldParserVerifierTests
{
    [TestClass]
    public class SimpleTests
    {
        Mock<IVerifierResultRepo> _resultRepo;

        private void WriteResults(VerificationResults resultSet)
        {
            Console.WriteLine("Succcess    : " + resultSet.Success);
            Console.WriteLine("Iterations  : " + resultSet.IterationCompleted);
            Console.WriteLine("ExecutionMS : " + resultSet.ExecutionTimeMS);

            foreach (var result in resultSet.Results)
            {
                Console.WriteLine($"Key:      {result.Key}");
                Console.WriteLine($" - Success:  {result.Success}");
                Console.WriteLine($" - Expected: {result.Expected}");
                Console.WriteLine($" - Actual: {result.Actual}");
                Console.WriteLine("  ");
            }

            foreach (var err in resultSet.ErrorMessage)
            {
                Console.WriteLine(err);
            }
        }


        [TestInitialize]
        public void Init()
        {
            _resultRepo = new Mock<IVerifierResultRepo>();
        }

        private IParserManager GetParserManager(string result)
        {
            var fakeParser = new Moq.Mock<IMessageFieldParser>();
            fakeParser.Setup(prs => prs.Parse(It.IsAny<PipelineExectionMessage>())).Returns(new IoT.Runtime.Core.Models.Messaging.MessageFieldParserResult()
            {
                Result = result,
                Success = true
               
            });

            var mockParserMgr = new Moq.Mock<IParserManager>();
            mockParserMgr.Setup(prs => prs.GetFieldMessageParser(It.IsAny<DeviceMessageDefinitionField>(), It.IsAny<ILogger>())).Returns(fakeParser.Object);
            return mockParserMgr.Object;
        }

        [TestMethod]
        public async Task Verfier_Field_Simple_Valid()
        {
            var parserMgr = GetParserManager("one");

            var config = new DeviceMessageDefinitionField();

            var verifier = new Verifier();
            verifier.VerifierType = EntityHeader<VerifierTypes>.Create(VerifierTypes.MessageFieldParser);
            verifier.ShouldSucceed = true;
            verifier.InputType = EntityHeader<InputTypes>.Create(InputTypes.Text);
            verifier.Input = "abc123";
            verifier.ExpectedOutput = "one";

            var verifiers = new FieldParserVerifierRuntime(parserMgr, _resultRepo.Object);
            var result = await verifiers.VerifyAsync(new IoT.Runtime.Core.Models.Verifiers.VerificationRequest<DeviceMessageDefinitionField>()
            {
                Verifier = verifier,
                Configuration = config
            }, null);

            WriteResults(result);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.IterationCompleted);
        }


        [TestMethod]
        public async Task Verfier_Field_Simple_Invalid()
        {
            var parserMgr = GetParserManager("two");

            var config = new DeviceMessageDefinitionField();

            var verifier = new Verifier();
            verifier.VerifierType = EntityHeader<VerifierTypes>.Create(VerifierTypes.MessageFieldParser);
            verifier.ShouldSucceed = true;
            verifier.InputType = EntityHeader<InputTypes>.Create(InputTypes.Text);
            verifier.Input = "abc123";
            verifier.ExpectedOutput = "one";

            var verifiers = new FieldParserVerifierRuntime(parserMgr, _resultRepo.Object);
            var result = await verifiers.VerifyAsync(new IoT.Runtime.Core.Models.Verifiers.VerificationRequest<DeviceMessageDefinitionField>()
            {
                Verifier = verifier,
                Configuration = config
            }, null);

            WriteResults(result);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.IterationCompleted);
        }



    }
}