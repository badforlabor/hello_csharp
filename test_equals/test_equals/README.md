# 结论

1. 重写Equals的同时，也要重写GetHashCode
2. list，dictionary等，默认是不会调用T的Equals的！
3. 纯结构体，不需要定义Equals，但是居然要定义GetHashCode
4. GetHashCode中，理论上，执行计算的成员变量，必须是readonly的，否则会有警告！
5. 提炼了CalcHash函数，在MyHashTool中。



# 常用

### MyHashTool

1. CombinHash，计算一组obj的HashCode
2. GetHashCode，List、Dict计算HashCode
3. Equals，List、Dict计算Equals