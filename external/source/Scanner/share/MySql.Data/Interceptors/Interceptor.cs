﻿// Copyright © 2004, 2011, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Text;
using mysql.Properties;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
  /// <summary>
  /// Interceptor is the base class for the "manager" classes such as ExceptionInterceptor,
  /// CommandInterceptor, etc
  /// </summary>
  internal abstract class Interceptor
  {
    protected MySqlConnection connection;

    protected void LoadInterceptors(string interceptorList)
    {
      if (String.IsNullOrEmpty(interceptorList)) return;

      string[] interceptors = interceptorList.Split('|');
      foreach (string interceptorType in interceptors)
      {
        if (String.IsNullOrEmpty(interceptorType)) continue;

        string type = ResolveType(interceptorType);
        Type t = Type.GetType(type);
        object interceptorObject = Activator.CreateInstance(t);
        AddInterceptor(interceptorObject);
      }
    }

    protected abstract void AddInterceptor(object o);

    protected virtual string ResolveType(string nameOrType)
    {
      return nameOrType;
    }
  }
}
