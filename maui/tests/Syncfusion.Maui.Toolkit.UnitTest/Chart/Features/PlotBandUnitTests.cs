using Syncfusion.Maui.Toolkit.Charts;

namespace Syncfusion.Maui.Toolkit.UnitTest
{
    public class PlotBandUnitTests: BaseUnitTest
    {
        [Fact(Skip = "need to check")]
        public void Swap_ValuesAreSwapped()
        {
            var numericalPlotBand = new NumericalPlotBand();
            float val1 = 5.0f;
            float val2 = 10.0f;
            object[] parameters = new object[] { val1, val2 };
            InvokePrivateMethod(numericalPlotBand, "Swap", parameters);
            Assert.Equal(10.0f, parameters[0]); 
            Assert.Equal(5.0f, parameters[1]);  
        }

        [Theory]
        [InlineData(10.0, 40.0, 30.0)] 
        [InlineData(50.0, 30.0, 20.0)] 

        public void GetBandWidth_VariousInputs_ReturnsExpectedWidth(double startBand, double end, double expected)
        {
            var numericalPlotBand = new NumericalPlotBand { End = end, Start=startBand };
            var result = (double?)InvokePrivateMethod(numericalPlotBand, "GetBandWidth", new object[] { startBand });
            Assert.Equal(expected, result); 
        }

        [Fact(Skip ="need to check")]
        public void GetBandWidth_NaNEndAndNullSize_ReturnsNaN()
        {
            var dateTimePlotBand = new DateTimePlotBand
            {
                End = new DateTime(2023,11,18), 
                Start = new DateTime(2022,11,12),
                Size=double.NaN,
            };
            double start = 44877;
            var expected = 371;
            
            var result = (double?)InvokePrivateMethod(dateTimePlotBand, "GetBandWidth", new object[] { start });
            Assert.Equal(expected,result);
        }
        [Fact]
        public void GetActualPeriodStrip_RepeatEveryIsNaN_ReturnsNaN()
        {
            var dateTimePlotBand = new DateTimePlotBand
            {
                RepeatEvery = double.NaN 
            };
            double start = 10;
            var result = (double?)InvokePrivateMethod(dateTimePlotBand, "GetActualPeriodStrip", new object[] { start });
            Assert.True(double.IsNaN(result ?? double.NaN)); 
        }
    }
}
