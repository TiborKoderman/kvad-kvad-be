public struct Val
{
    public double? m_double { get; set; }
    public long? m_long { get; set; }
    public bool? m_bool { get; set; }
    public string? m_string { get; set; }

    public object? GetValue()
    {
        if (m_double.HasValue) return m_double.Value;
        if (m_long.HasValue) return m_long.Value;
        if (m_bool.HasValue) return m_bool.Value;
        if (m_string != null) return m_string;
        return null;
    }
}