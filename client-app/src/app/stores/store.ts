import { createContext, useContext } from "react";
import ActivityStore from "./activityStore";
import CommonStore from "./commonStore";

interface Store{
    activityStore: ActivityStore;
    commonStore: CommonStore;
}
// her we add all the store
export const store: Store = {
    activityStore: new ActivityStore(),
    commonStore: new CommonStore()
}
// and from the react context we can access our stores
export const StoreContext = createContext(store);

export function useStore(){
    return useContext(StoreContext);
}