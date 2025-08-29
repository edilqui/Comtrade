using Axon.Comtrade.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

public class FolderModel
{
    public string Name { get; set; }
   
    public FolderModel Parent { get; set; } // puede ser null en la raíz

    public List<FolderModel> Subfolders { get; set; } = new List<FolderModel>();
    public ObservableCollection<ArchivedFilterModel> ArchivedFilters { get; set; }

    public FolderModel(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Folder name cannot be null or whitespace.", nameof(name));

        Name = name.Trim();
        ArchivedFilters = new ObservableCollection<ArchivedFilterModel>() { new ArchivedFilterModel() };
    }

    public FolderModel()
    {
        ArchivedFilters = new ObservableCollection<ArchivedFilterModel>() { new ArchivedFilterModel() };
    }

    // --- Gestión de subcarpetas ---
    public FolderModel AddSubfolder(string name)
    {
        // Opcional: evitar duplicados por nombre (case-insensitive)
        var existing = Subfolders.Find(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
        if (existing != null) return existing;

        var child = new FolderModel(name);
        child.Parent = this;
        Subfolders.Add(child);
        return child;
    }

    public bool RemoveSubfolder(string name)
    {
        var idx = Subfolders.FindIndex(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
        if (idx < 0) return false;

        Subfolders[idx].Parent = null;
        Subfolders.RemoveAt(idx);
        return true;
    }

    public FolderModel Find(string name)
    {
        if (string.Equals(Name, name, StringComparison.OrdinalIgnoreCase)) return this;

        foreach (var child in Subfolders)
        {
            var found = child.Find(name);
            if (found != null) return found;
        }
        return null;
    }

    // Devuelve la ruta relativa desde la raíz del modelo
    public string GetRelativePath(string separator = null)
    {
        if (separator == null) separator = Path.DirectorySeparatorChar.ToString();

        var stack = new Stack<string>();
        var node = this;
        while (node != null)
        {
            stack.Push(node.Name);
            node = node.Parent;
        }
        return string.Join(separator, stack.ToArray());
    }

    // --- Crear estructura en disco ---
    public void CreateOnDisk(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentException("Base path is required.", nameof(basePath));

        var current = Path.Combine(basePath, Name);
        Directory.CreateDirectory(current);

        foreach (var child in Subfolders)
            child.CreateOnDisk(current);
    }

    // --- Recorridos útiles ---
    public IEnumerable<FolderModel> TraverseDepthFirst()
    {
        yield return this;

        foreach (var child in Subfolders)
        {
            foreach (var n in child.TraverseDepthFirst())
                yield return n;
        }
    }

    public IEnumerable<FolderModel> TraverseBreadthFirst()
    {
        var q = new Queue<FolderModel>();
        q.Enqueue(this);

        while (q.Count > 0)
        {
            var node = q.Dequeue();
            yield return node;

            foreach (var c in node.Subfolders)
                q.Enqueue(c);
        }
    }

    // --- Serialización / deserialización (Newtonsoft.Json) ---
    //public string ToJson(bool indented = true)
    //{
    //    var formatting = indented ? Formatting.Indented : Formatting.None;
    //    return JsonConvert.SerializeObject(this, formatting);
    //}

    //public static FolderModel FromJson(string json)
    //{
    //    var model = JsonConvert.DeserializeObject<FolderModel>(json);
    //    if (model == null) throw new InvalidDataException("Invalid JSON for FolderModel.");

    //    RehydrateParents(model, null);
    //    return model;
    //}

    private static void RehydrateParents(FolderModel node, FolderModel parent)
    {
        node.Parent = parent;
        foreach (var child in node.Subfolders)
            RehydrateParents(child, node);
    }

    // --- Construir el modelo desde un directorio real ---
    public static FolderModel FromDisk(string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException(path);

        var rootName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var root = new FolderModel(rootName);

        BuildFromDisk(root, path);
        return root;
    }

    private static void BuildFromDisk(FolderModel node, string fullPath)
    {
        var dirs = Directory.GetDirectories(fullPath);
        foreach (var dir in dirs)
        {
            var name = Path.GetFileName(dir);
            var child = node.AddSubfolder(name);
            BuildFromDisk(child, dir);
        }
    }

    internal void AddFilter()
    {
        this.ArchivedFilters.Add(new ArchivedFilterModel());
    }
}
