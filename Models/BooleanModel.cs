using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Utility.WPF.Services.Demo
{
    public enum ApplicationStatus
    {
        Accepted,
        Rejected
    }

    public class Global : Service<ContinueRequest, ContinueApplication, GlobalResponse>
    {
        public const string NetworkKey = "1";
        public const string GlobalKey = "1";

        public Global()
        {
            this.Key = GlobalKey;
        }

        public static Global Instance { get; } = new Global();

        //public ObservableCollection<ContinueRequest> WaitingForContinueValues { get; } = new();

        protected override GlobalResponse Convert(ContinueRequest @in, ContinueApplication continueApplication)
        {
            switch (continueApplication.Status)
            {
                case ApplicationStatus.Accepted:
                    return new ContinuePermit(@in.Key);

                case ApplicationStatus.Rejected:
                    return new ContinueRejection(@in.Key);
            }
            throw new Exception("FDS fr 34");
        }

        //case ExceptionValue { Key: var key } val:
        //    {
        //        Outputs.Add(val);
        //        return val;
        //    }

        // throw new Exception("s  77df3");
    }

    public record ContinueRequest(string Key) : Value;//(string Key) : GlobalValue(Key);
    public record ContinueApplication(string Key, ApplicationStatus Status) : Box<ApplicationStatus>(Status);//(string Key) : GlobalValue(Key);
    public record ContinuePermit(string Key) : GlobalResponse(Key);//(string Key) : GlobalValue(Key);
    public record ContinueRejection(string Key) : GlobalResponse(Key);//(string Key) : GlobalValue(Key);
    public record GlobalResponse(string Key) : Value;//(string Key) : GlobalValue(Key);
    public record GlobalRequest(string Key) : Value;//(string Key) : GlobalValue(Key);
    public record WaitResponse(string Key) : GlobalResponse(Key);//(string Key) : GlobalValue(Key);

    public class Connection<T> : Connection
    {
    }

    public class ToGlobalConnection : Connection<GlobalRequest>
    { }

    public class FromGlobalConnection : Connection<GlobalResponse>
    { }

    public class Connection : IObserver<Message>, IObservable<Message>
    {
        private IObserver<Message>? observer;

        public Connection()
        {
        }

        public Message? Message { get; private set; }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Message value)
        {
            this.Message = value;
            if (observer != null)
                observer.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (this.observer != null)
                throw new Exception("df .;ksfs");
            this.observer = observer;
            if (Message != null)
                observer.OnNext(Message);
            return new Disposable(new ObservableCollection<IObserver<Message>>(new[] { observer }), observer);
        }
    }

    public enum Response
    {
        Accept, Reject
    }

    public record ProceedResponse(Response Response) : Box<Response>(Response);

    public record Box<T>(T Value) : Value;
    public record Value();

    public record Message(string Source, string Destination, string NetworkKey, Value Value, params Message[] Children);

    public class Base : IObservable<Message>, IDisposable
    {
        //private ObservableCollection<Value> outputs = new();
        private ObservableCollection<IObserver<Message>> observers = new();

        public ObservableCollection<IObserver<Message>> Connections => observers;
        //public ObservableCollection<Value> Outputs => outputs;

        public void Dispose()
        {
            observers.Clear();
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            //foreach (var output in outputs)
            //    observer.OnNext(new Message(output));
            return new Disposable(observers, observer);
        }
    }

    public class Disposable : IDisposable
    {
        private readonly ObservableCollection<IObserver<Message>> observers;
        private readonly IObserver<Message> observer;

        public Disposable(ObservableCollection<IObserver<Message>> observers, IObserver<Message> observer)
        {
            observers.Add(observer);
            this.observers = observers;
            this.observer = observer;
        }

        public ObservableCollection<IObserver<Message>> Observers => observers;
        public IObserver<Message> Observer => observer;

        public void Dispose()
        {
            observers.Remove(observer);
        }
    }

    public abstract class Service<TIn1, TIn2, TOut> : Base, IObserver<Message>
    //where TIn : Box
    //where TOut : Box
    {
        private ObservableCollection<Value> pending = new();
        //private ObservableCollection<Value> values = new();

        public Service()
        {
        }

        //public ObservableCollection<Value> Values => values;

        public ObservableCollection<Value> Pending => pending;
        public ObservableCollection<Func<Value>> PendingActions { get; } = new();

        //public ObservableCollection<IObserver<Message>> Queue { get; } = new();

        public string Key { get; init; }

        public void OnCompleted()
        {
            throw new NotImplementedException();
            //Global.Instance.OnNext(new Message(this.Key, Global.Instance.Key, 1, new Completed(this.Key)));
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
            //Global.Instance.OnNext(new Message(new ExceptionValue(this.Key)));
        }

        protected abstract TOut Convert(TIn1 @in1, TIn2 @in2);

        protected virtual TIn1 Convert(TIn2 @in1, TOut @in2)
        {
            throw new NotImplementedException();
        }

        protected virtual TIn2 Convert(TIn1 @in1, TOut @in2)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, Value> dictionary = new();

        public void OnNext(Message message)
        {
            switch (message.Value)
            {
                case ProceedResponse { Response: Response.Accept }:
                    {
                        Proceed();
                        Remove();
                        break;
                    }
                case ProceedResponse { Response: Response.Reject }:
                    {
                        throw new Exception("DSFG Sss");
                    }
                case Value value:
                    {
                        dictionary[message.Source] = value;
                        if (dictionary.Count >= 2)
                        {
                            foreach (var item in dictionary)
                            {
                                if (item.Key != message.Source)
                                {
                                    value = TryConvert(new[] { dictionary[message.Source], item.Value });
                                    Pending.Add(value);
                                }
                            }
                        }
                        Remove();
                        break;
                    }
            }
            //  Global.Instance.OnNext(new Message(this.Key, Global.GlobalKey, Global.NetworkKey, Value: new ContinueRequest(this.Key)));
        }

        private void Proceed()
        {
            if (Pending.Any())
            {
                var last = Pending.Last();
                Pending.RemoveAt(Pending.Count - 1);
                var (a, b) = NewMethod(last);
                a.OnNext(new Message(this.Key, Global.GlobalKey, Global.NetworkKey, Value: b));
            }
            else
            {
                throw new Exception("DFg dfgg");
            }
        }

        private void Remove()
        {
            if (Pending.Any())
            {
                Connections.Single(a => a is ToGlobalConnection)
                   .OnNext(new Message(this.Key, Global.GlobalKey, Global.NetworkKey, Value: new ContinueRequest(this.Key)));
            }
        }

        private (Connection, Value) NewMethod(Value last)
        {
            foreach (var connection in Connections)
            {
                switch (connection, last)
                {
                    case (Connection<TIn1> conn, TIn1):
                        return (conn, last);

                    case (Connection<TIn2> conn, TIn2):
                        return (conn, last);

                    case (Connection<TOut> conn, TOut):
                        return (conn, last);
                }
            }
            throw new Exception("sdf s sd");
        }

        public virtual Value? TryConvert(IList<Value> values)
        {
            TOut output;

            try
            {
                if (values.Count > 2)
                {
                    throw new Exception("dfvg");
                }
                switch (values[0], values[1])
                {
                    case (Box<TIn1> box1, Box<TIn2> box2):
                        return new Box<TOut>(Convert(box1.Value, box2.Value));

                    case (Box<TIn2> box2, Box<TIn1> box1):
                        return new Box<TOut>(Convert(box1.Value, box2.Value));

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            throw new NotImplementedException();
        }
    }


}