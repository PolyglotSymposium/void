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
