using System.Collections.Generic;

namespace KNXIntegrator;
public class Publisher{

    private List<Subscriber> subscribers;
    public void AddSubscriber(Subscriber subscriber){
        subscribers.Add(subscriber);
    }

    public void RemoveSubscriber(Subscriber subscriber){
        subscribers.Remove(subscriber);
    }

    public void notifySubscribers(params object[] items){
        foreach (Subscriber subscriber in subscribers){
            subscriber.update(items);
        }
    }
}