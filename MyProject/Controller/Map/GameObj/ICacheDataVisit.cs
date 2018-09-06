using System;
using System.Collections.Generic;

public interface ICacheDataVisit<KEY, VALUE>
{
    //VALUE Pull(KEY key);
    void Flush(KEY key, VALUE data);
}
public interface IPoolVisit<KEY, VALUE>
{
    bool Exist(KEY key);
    VALUE Get(KEY key);
}