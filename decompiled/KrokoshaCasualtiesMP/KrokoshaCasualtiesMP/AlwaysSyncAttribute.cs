using System;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public class AlwaysSyncAttribute : DoSyncAttribute
{
}
