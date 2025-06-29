namespace MagicDeckStats.Services;

public interface IGlobalFilterService
{
    event Action? OnFilterChanged;
    HashSet<string> SelectedVariants { get; }
    void SetVariantFilter(HashSet<string> variants);
    void SetAllAvailableVariants(IEnumerable<string> variants);
    void ClearVariantFilter();
    bool IsVariantSelected(string variant);
}

public class GlobalFilterService : IGlobalFilterService
{
    private HashSet<string> _selectedVariants = [];

    public event Action? OnFilterChanged;

    public HashSet<string> SelectedVariants => [.. _selectedVariants];

    public void SetVariantFilter(HashSet<string> variants)
    {
        _selectedVariants = new HashSet<string>(variants, StringComparer.OrdinalIgnoreCase);
        OnFilterChanged?.Invoke();
    }

    public void SetAllAvailableVariants(IEnumerable<string> variants)
    {
        _selectedVariants = new HashSet<string>(variants, StringComparer.OrdinalIgnoreCase);
        OnFilterChanged?.Invoke();
    }

    public void ClearVariantFilter()
    {
        _selectedVariants.Clear();
        OnFilterChanged?.Invoke();
    }

    public bool IsVariantSelected(string variant)
    {
        return _selectedVariants.Contains(variant);
    }
} 