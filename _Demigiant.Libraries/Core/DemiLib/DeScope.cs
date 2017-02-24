// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/24 18:37
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib
{
    /// <summary>
    /// Extend this to replicate Unity's Scope system with any Unity version.
    /// Thanks to Dmitriy Yukhanov for pointing this out and creating an initial version.
    /// Expand this class to create scopes.<para/>
    /// Example:
    /// <code>public class VBoxScope : DeScope
    /// {
    ///     public VBoxScope(GUIStyle style)
    ///     {
    ///         BeginVBox(style);
    ///     }
    ///
    ///     protected override void CloseScope()
    ///     { 
    ///         EndVBox();
    ///     }
    /// }</code>
    /// Usage:
    /// <code>using (new VBoxScope(myStyle) {
    ///     // Do something
    /// }</code>
    /// </summary>
    public abstract class DeScope : IDisposable
    {
        bool _disposed;

	    protected abstract void CloseScope();

	    ~DeScope()
	    {
	        if (_disposed) return;
	        Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
	        Dispose();
	    }

	    public void Dispose()
	    {
		    if (_disposed) return;
		    _disposed = true;
		    CloseScope();
	    }
    }
}