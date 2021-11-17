import { createContext, useContext } from "react";
import ActivityStore from "./activityStore";
import CommentStore from "./commentStore";
import CommonStore from "./commonStore";
import ModalStore from "./modalStore";
import ProfileStore from "./profileStore";
import UserStore from "./userStore";

interface Store{
    activityStore: ActivityStore;
    commonStore: CommonStore;
    userStore: UserStore;
    modalStore : ModalStore;
    profileStore : ProfileStore;
    commentStore: CommentStore;
}
// her we add all the store
export const store: Store = {
    activityStore: new ActivityStore(),
    commonStore: new CommonStore(),
    userStore: new UserStore(),
    modalStore: new ModalStore(),
    profileStore : new ProfileStore(),
    commentStore: new CommentStore()
}
// and from the react context we can access our stores
export const StoreContext = createContext(store);

export function useStore(){
    return useContext(StoreContext);
}