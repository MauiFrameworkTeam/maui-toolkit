﻿namespace Syncfusion.Maui.Toolkit.Charts
{
    /// <summary>
    /// The <see cref="StackingLine100Series"/> is a collection of data points, where the lines are stacked on top of each other to represent a percentage of the total for each category.
    /// </summary>
    /// <remarks>
    /// <para>The cumulative portion of each stacked element always total to 100% </para>
    /// <para>To render a series, create an instance of <see cref="StackingLine100Series"/> class, and add it to the <see cref="SfCartesianChart.Series"/> collection.</para>
    /// <para>It provides options for <see cref="ChartSeries.Fill"/>, <see cref="ChartSeries.PaletteBrushes"/>, <see cref="XYDataSeries.StrokeWidth"/>,  <see cref="StackingSeriesBase.StrokeDashArray"/> and <see cref="ChartSeries.Opacity"/> to customize the appearance.</para>
    /// <para>Utilize the <see cref="ChartSeries.Fill"/> property to customize the line stroke.</para>
    /// <para> <b>EnableTooltip - </b> A tooltip displays information while tapping or mouse hovering above a segment. To display the tooltip on a chart, you need to set the <see cref="ChartSeries.EnableTooltip"/> property as <b>true</b> in <see cref="StackingLine100Series"/> class, and also refer <seealso cref="ChartBase.TooltipBehavior"/> property.</para>
    /// <para> <b>Data Label - </b> Data labels are used to display values related to a chart segment. To render the data labels, you need to set the <see cref="ChartSeries.ShowDataLabels"/> property as <b>true</b> in <see cref="StackingLine100Series"/> class. To customize the chart data labels placement, and label styles, you need to create an instance of <see cref="CartesianDataLabelSettings"/> and set to the <see cref="CartesianSeries.DataLabelSettings"/> property.</para>
    /// <para> <b>Animation - </b> To animate the series, set <b>True</b> to the <see cref="ChartSeries.EnableAnimation"/> property.</para>
    /// <para> <b>LegendIcon - </b> To customize the legend icon using the <see cref="ChartSeries.LegendIcon"/> property.</para>
    /// </remarks>
    /// <example>
    /// # [Xaml](#tab/tabid-1)
    /// <code><![CDATA[
    /// <chart:SfCartesianChart>
    ///     <chart:SfCartesianChart.XAxes>
    ///         <chart:CategoryAxis/>
    ///     </chart:SfCartesianChart.XAxes>
    ///
    ///     <chart:SfCartesianChart.YAxes>
    ///         <chart:NumericalAxis/>
    ///     </chart:SfCartesianChart.YAxes>
    ///
    ///     <chart:StackingLine100Series
    ///         ItemsSource = "{Binding MedalDetails}"
    ///         XBindingPath = "CountryName"
    ///         YBindingPath = "GoldMedals"/>
    ///
    ///     <chart:StackingLine100Series
    ///         ItemsSource = "{Binding MedalDetails}"
    ///         XBindingPath = "CountryName"
    ///         YBindingPath = "SilverMedals"/>
    ///
    ///     <chart:StackingLine100Series
    ///         ItemsSource = "{Binding MedalDetails}"
    ///         XBindingPath = "CountryName"
    ///         YBindingPath = "BronzeMedals"/>
    /// </chart:SfCartesianChart>
    /// ]]></code>
    /// # [C#](#tab/tabid-2)
    /// <code><![CDATA[
    /// SfCartesianChart chart = new SfCartesianChart();
    ///
    /// CategoryAxis xAxis = new CategoryAxis();
    /// NumericalAxis yAxis = new NumericalAxis();
    ///
    /// chart.XAxes.Add(xAxis);
    /// chart.YAxes.Add(yAxis);
    ///
    /// ViewModel viewModel = new ViewModel();
    ///
    /// StackingLine100Series goldSeries = new StackingLine100Series();
    /// goldSeries.ItemsSource = viewModel.MedalDetails;
    /// goldSeries.XBindingPath = "CountryName";
    /// goldSeries.YBindingPath = "GoldMedals";
    ///
    /// StackingLine100Series silverSeries = new StackingLine100Series();
    /// silverSeries.ItemsSource = viewModel.MedalDetails;
    /// silverSeries.XBindingPath = "CountryName";
    /// silverSeries.YBindingPath = "SilverMedals";
    ///
    /// StackingLine100Series bronzeSeries = new StackingLine100Series();
    /// bronzeSeries.ItemsSource = viewModel.MedalDetails;
    /// bronzeSeries.XBindingPath = "CountryName";
    /// bronzeSeries.YBindingPath = "BronzeMedals";
    ///
    /// chart.Series.Add(goldSeries);
    /// chart.Series.Add(silverSeries);
    /// chart.Series.Add(bronzeSeries);
    ///
    /// this.Content = chart;
    ///     
    /// ]]></code>
    /// # [ViewModel](#tab/tabid-3)
    /// <code><![CDATA[
    ///     public ObservableCollection<MedalData> MedalDetails { get; set; }
    ///
    ///     public ViewModel()
    ///     {
    ///         MedalDetails = new ObservableCollection<MedalData>
    ///         {
    ///             new MedalData() { CountryName = "USA", GoldMedals = 10, SilverMedals = 5, BronzeMedals = 7 },
    ///             new MedalData() { CountryName = "China", GoldMedals = 8, SilverMedals = 10, BronzeMedals = 6 },
    ///             new MedalData() { CountryName = "Russia", GoldMedals = 6, SilverMedals = 4, BronzeMedals = 8 },
    ///             new MedalData() { CountryName = "UK", GoldMedals = 4, SilverMedals = 7, BronzeMedals = 3 }
    ///         };
    ///     }
    /// ]]></code>
    /// ***
    /// </example>
    public class StackingLine100Series : StackingLineSeries
    {
        #region Internal Method

        internal override void UpdateRange()
        {
            double yStart = YRange.Start;
            double yEnd = YRange.End;

            YRange = new DoubleRange(yStart, yEnd);
            base.UpdateRange();
        }

        #endregion
    }
}