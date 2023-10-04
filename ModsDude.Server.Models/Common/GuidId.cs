using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueOf;

namespace ModsDude.Server.Models.Common;
public class GuidId<T> : ValueOf<Guid, T>
    where T : ValueOf<Guid, T>, new()
{
    public static T NewId()
    {
        return From(Guid.NewGuid());
    }
}
