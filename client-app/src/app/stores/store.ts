import { createContext, useContext } from "react";
import ActivityStore from "./activityStore";

interface Store{
    activityStore: ActivityStore
}
// her we add all the store
export const store: Store = {
    activityStore: new ActivityStore()
}
// and from the react context we can access our stores
export const StoreContext = createContext(store);

export function useStore(){
    return useContext(StoreContext);
}