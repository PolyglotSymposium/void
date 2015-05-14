namespace Void.Util

module SeqUtil =
    let notMoreThan count (source : seq<'T>) =
        seq {
            use enumerator = source.GetEnumerator()
            let i = ref 0
            while enumerator.MoveNext() && !i < count do
                i := !i + 1
                yield enumerator.Current
        }

module StringUtil =
    let noLongerThan length (text : string) =
        if text.Length > length
        then text.Substring(0, length)
        else text

(* Overcome the impedance mismatch between F# and C# functions.
 * Thanks to Jared Parsons.
 * http://blogs.msdn.com/b/jaredpar/archive/2010/07/27/converting-system-func-lt-t1-tn-gt-to-fsharpfunc-lt-t-tresult-gt.aspx
 * *)
 open System.Runtime.CompilerServices
[<Extension>]
type public FSharpFuncUtil = 
    [<Extension>] 
    static member ToFSharpFunc<'a,'b> (func:System.Func<'a,'b>) = fun x -> func.Invoke(x)
    static member Create<'a,'b> (func:System.Func<'a,'b>) = FSharpFuncUtil.ToFSharpFunc func
