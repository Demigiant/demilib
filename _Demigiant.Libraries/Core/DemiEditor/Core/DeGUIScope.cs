// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/02/20 12:00
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiEditor.Core
{
    /// <summary>
    /// Replicates Unity's GUI.Scope with Unity versions older than 5.X.
    /// Thanks to Dmitriy Yukhanov for pointing this out and creating an initial version
    /// (which I meant to use until we discovered Unity had implemented it too)
    /// </summary>
    public abstract class DeGUIScope : IDisposable
    {
	    bool _disposed;

	    protected abstract void CloseScope();

	    ~DeGUIScope()
	    {
		    if (!_disposed) {
			    Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
			    Dispose();
		    }
	    }

	    public void Dispose()
	    {
		    if (_disposed) return;

		    _disposed = true;
		    CloseScope();
	    }
    }
}