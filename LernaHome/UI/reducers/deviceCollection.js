import {deviceActions} from "../actions";

const defaultState = {
    "http://www.w3.org/ns/hydra/core#member": []
};

const deviceCollection = (state = defaultState, action) => {
    switch (action.type) {
        case deviceActions.DEVICES_COLLECTION_GET:
            return action.deviceCollection;
        default:
            return state;
    }
};

export default deviceCollection;