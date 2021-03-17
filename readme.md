# IsoDuration

Parses an ISO-8601 duration string to `IsoDuration` instance. The `IsoDuration` can be added to a `DateTimeOffset` or `DateTime` value to obtain an actual date result. Some durations are not exact (see `IsoDuration.IsAmbigious` and `IsoDuration.IsExact`), since the number of days in a year changes based on leap year, the number of days in a month changes, and Daylight Saving Time (DST) can impact the number of hours in a day.

## Examples

Here are some interval examples:

```
"P4Y",
"P1DT2H3M4S"
"PT12,345S",
"-P3Y6M4DT12H30M5.123S",
```

# IsoInterval

Parses an ISO-8601 interval string to `IsoInterval` instance.

## Examples

Here are some interval examples:

```
"2007-03-01T13:00:00Z/2008-05-11T15:30:00Z",
"2007-03-01T13:00:00Z/P1Y2M10DT2H30M",
"P1Y2M10DT2H30M/2008-05-11T15:30:00Z",
"P1Y2M10DT2H30M",
```