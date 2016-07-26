namespace PrintTests
{
    using Print;
    using Xunit;
    using Moq;
    using Macprint.Interfaces;

    public class PrintUtilsTests
    {
        private readonly Mock<ILog> mockLogger = new Mock<INLogger>();
        private readonly Mock<IPrintUtilsHelper> mockPrinterHelper = new Mock<IPrintUtilsHelper>();
        private PrintUtils PrintUtility { get; set; }

        [Fact]
        public void PrintWitAdobeExtensionCalledOnceWhenExtensionIsPdf()
        {
            mockPrinterHelper.Setup(x => x.PrintWithAdobe(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var args = new string[] { "test.pdf", @"\\ms35\ricoh7", "true"};
            PrintUtility = new PrintUtils(mockLogger.Object, mockPrinterHelper.Object, args);

            PrintUtility.Print();

           mockPrinterHelper.Verify(x => x.PrintWithAdobe(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PrintWitAdobeExtensionCalledZeroTimesWhenExtensionIsNotPdf()
        {
            mockPrinterHelper.Setup(x => x.PrintWithAdobe(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var args = new string[] { "test.xml", @"\\ms35\ricoh7", "true" };
            PrintUtility = new PrintUtils(mockLogger.Object, mockPrinterHelper.Object, args);

            PrintUtility.Print();

            mockPrinterHelper.Verify(x => x.PrintWithAdobe(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void PrintWitLableExtensionCalledOnceWhenExtensionIsLable()
        {
            mockPrinterHelper.Setup(x => x.SendFileToPrinter(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var args = new string[] { "test.lable", @"\\ms35\ricoh7", "true" };
            PrintUtility = new PrintUtils(mockLogger.Object, mockPrinterHelper.Object, args);

            PrintUtility.Print();

            mockPrinterHelper.Verify(x => x.SendFileToPrinter(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PrintWitLableCalledZeroTimesWhenExtensionIsNotLable()
        {
            mockPrinterHelper.Setup(x => x.SendFileToPrinter(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var args = new string[] { "test.xml", @"\\ms35\ricoh7", "true" };
            PrintUtility = new PrintUtils(mockLogger.Object, mockPrinterHelper.Object, args);

            PrintUtility.Print();

            mockPrinterHelper.Verify(x => x.SendFileToPrinter(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void PrintWitFlatFileCalledOncesWhenExtensionIsNotPdfOrNotLable()
        {
            mockPrinterHelper.Setup(x =>x.PrintNoextentionFile(It.IsAny<string>(), true, It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>(), false, true)).Verifiable();

            var args = new string[] { "test.xml", "true", @"\\ms35\ricoh7", "A4", "10", "source", "false", "true" };

            PrintUtility = new PrintUtils(mockLogger.Object, mockPrinterHelper.Object, args);
            PrintUtility.Print();

            mockPrinterHelper.Verify(x => x.PrintNoextentionFile(It.IsAny<string>(), true, It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>(), false, true), Times.Once());
        }
    }
}
