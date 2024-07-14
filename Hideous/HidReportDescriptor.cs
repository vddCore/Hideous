using Hideous.DescriptorParser;
using Hideous.DescriptorParser.Items;

namespace Hideous
{
    public sealed class HidReportDescriptor
    {
        private readonly HidDevice _device;
        private List<CollectionItem> _topLevelCollections;

        private readonly List<HidReport> _allReports = new();
        
        private readonly Dictionary<byte, HidInputReport> _idInputReports = new();
        private readonly Dictionary<byte, HidOutputReport> _idOutputReports = new();
        private readonly Dictionary<byte, HidFeatureReport> _idFeatureReports = new();

        public IReadOnlyList<HidReport> Reports => _allReports;

        public IReadOnlyDictionary<byte, HidInputReport> InputReports => _idInputReports;
        public IReadOnlyDictionary<byte, HidOutputReport> OutputReports => _idOutputReports;
        public IReadOnlyDictionary<byte, HidFeatureReport> FeatureReports => _idFeatureReports;

        internal HidReportDescriptor(HidDevice device, byte[] rawDescriptor)
        {
            _device = device;
            _topLevelCollections = new DescriptorReader(
                rawDescriptor
            ).ReadRawDescriptor();

            foreach (var collectionItem in _topLevelCollections)
            {
                var traversalState = new TraversalState();
                traversalState.AssumeItemCharacteristics(collectionItem);
                TraverseCollectionItem(collectionItem, traversalState);
            }
        }

        private void TraverseCollectionItem(CollectionItem item, TraversalState state)
        {
            foreach (var subItem in item.Items)
            {
                state.AssumeItemCharacteristics(subItem);

                if (subItem is CollectionItem subCollection)
                {
                    TraverseCollectionItem(subCollection, state);
                }
                else if (subItem is InputItem)
                {
                    if (state.HasReportId)
                    {
                        var reportId = (byte)state.ReportId.Value;
                        var report = new HidInputReport(
                            _device,
                            reportId,
                            state.ReportSize.Value,
                            state.ReportCount.Value,
                            (ushort)state.UsageId.Value,
                            (ushort)state.UsagePage.Value
                        );

                        if (_idInputReports.TryAdd(reportId, report))
                        {
                            _allReports.Add(report);
                        }
                        else
                        {
                            _idInputReports[reportId].AddUsagesFromItem(subItem);
                        }
                    }
                    else
                    {
                        _allReports.Add(new HidInputReport(
                            _device,
                            state.ReportSize.Value,
                            state.ReportCount.Value,
                            (ushort)state.UsageId.Value,
                            (ushort)state.UsagePage.Value
                        ));
                    }
                }
                else if (subItem is OutputItem)
                {
                    if (state.HasReportId)
                    {
                        var reportId = (byte)state.ReportId.Value;
                        var report = new HidOutputReport(
                            _device,
                            reportId,
                            state.ReportSize.Value,
                            state.ReportCount.Value,
                            (ushort)state.UsageId.Value,
                            (ushort)state.UsagePage.Value
                        );

                        if (_idOutputReports.TryAdd(reportId, report))
                        {
                            _allReports.Add(report);
                        }
                        else
                        {
                            _idOutputReports[reportId].AddUsagesFromItem(subItem);
                        }
                    }
                    else
                    {
                        _allReports.Add(new HidOutputReport(
                            _device,
                            state.ReportSize.Value,
                            state.ReportCount.Value,
                            (ushort)state.UsageId.Value,
                            (ushort)state.UsagePage.Value
                        ));
                    }
                }
                else if (subItem is FeatureItem)
                {
                    if (state.HasReportId)
                    {
                        var reportId = (byte)state.ReportId.Value;
                        var report = new HidFeatureReport(
                            _device,
                            reportId,
                            state.ReportSize.Value,
                            state.ReportCount.Value,
                            (ushort)state.UsageId.Value,
                            (ushort)state.UsagePage.Value
                        );

                        if (_idFeatureReports.TryAdd(reportId, report))
                        {
                            _allReports.Add(report);
                        }
                        else
                        {
                            _idFeatureReports[reportId].AddUsagesFromItem(subItem);
                        }
                    }
                    else
                    {
                        _allReports.Add(new HidFeatureReport(
                            _device,
                            state.ReportSize.Value,
                            state.ReportCount.Value,
                            (ushort)state.UsageId.Value,
                            (ushort)state.UsagePage.Value
                        ));
                    }
                }
            }
        }

        private class TraversalState
        {
            public bool HasReportId => ReportId != null!;

            public Item UsageId { get; set; } = null!;
            public Item UsagePage { get; set; } = null!;
            public Item ReportId { get; set; } = null!;
            public Item ReportSize { get; set; } = null!;
            public Item ReportCount { get; set; } = null!;

            public void AssumeItemCharacteristics(Item item)
            {
                UsageId = item[LocalTag.Usage] ?? UsageId;
                UsagePage = item[GlobalTag.UsagePage] ?? UsagePage;
                ReportId = item[GlobalTag.ReportId]!;
                ReportSize = item[GlobalTag.ReportSize] ?? ReportSize;
                ReportCount = item[GlobalTag.ReportCount] ?? ReportCount;
            }
        }
    }
}