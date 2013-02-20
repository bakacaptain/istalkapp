using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Interface to be implemented by classes that can be converted to XML.
    /// </summary>
    public interface IXmlable
    {
        String ToXML();
    }
}
