using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.TestTools;
using Assembly = System.Reflection.Assembly;

namespace Course.Utility.Events
{
  /// <summary>
  /// Utility class for retrieving predefined assemblies and their types.
  /// Provides methods to filter and extract types implementing a specific interface.
  /// </summary>
  [ExcludeFromCoverage]
  [ExcludeFromCodeCoverage]
  public static class PredefinedAssemblyUtility
  {
    /// <summary>
    /// Enum representing different assembly types in the Unity project.
    /// </summary>
    private enum AssemblyType
    {
      /// <summary>
      /// Represents the main Assembly-CSharp assembly.
      /// </summary>
      AssemblyCSharp,

      /// <summary>
      /// Represents the Assembly-CSharp-Editor assembly.
      /// </summary>
      AssemblyCSharpEditor,

      /// <summary>
      /// Represents the Assembly-CSharp-Editor-firstpass assembly.
      /// </summary>
      AssemblyCSharpEditorFirstPass,

      /// <summary>
      /// Represents the Assembly-CSharp-firstpass assembly.
      /// </summary>
      AssemblyCSharpFirstPass,
    }

    /// <summary>
    /// Determines the type of assembly based on its name.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly.</param>
    /// <returns>The corresponding <see cref="AssemblyType"/> or null if the name does not match.</returns>
    private static AssemblyType? GetAssemblyType(string assemblyName)
    {
      return assemblyName switch
      {
        "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
        "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
        "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
        "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
        _ => null,
      };
    }

    /// <summary>
    /// Adds types from a given assembly to the results collection if they implement the specified interface.
    /// </summary>
    /// <param name="assemblyTypes">Array of types from the assembly.</param>
    /// <param name="interfaceType">The interface type to filter by.</param>
    /// <param name="results">The collection to which matching types will be added.</param>
    private static void AddTypesFromAssembly(
      Type[] assemblyTypes,
      Type interfaceType,
      ICollection<Type> results
    )
    {
      if (assemblyTypes == null)
        return;

      for (int i = 0; i < assemblyTypes.Length; i++)
      {
        Type type = assemblyTypes[i];
        if (type != interfaceType && interfaceType.IsAssignableFrom(type))
        {
          results.Add(type);
        }
      }
    }

    /// <summary>
    /// Retrieves all types from predefined assemblies that implement the specified interface.
    /// </summary>
    /// <param name="interfaceType">The interface type to filter by.</param>
    /// <returns>A list of types implementing the specified interface.</returns>
    public static List<Type> GetTypes(Type interfaceType)
    {
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

      Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
      List<Type> types = new List<Type>();

      for (int i = 0; i < assemblies.Length; i++)
      {
        AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
        if (assemblyType != null)
        {
          assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
        }
      }

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
      AddTypesFromAssembly(assemblyCSharpTypes, interfaceType, types);

      assemblyTypes.TryGetValue(
        AssemblyType.AssemblyCSharpFirstPass,
        out var assemblyCSharpFirstPassTypes
      );
      AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interfaceType, types);

      return types;
    }
  }
}
