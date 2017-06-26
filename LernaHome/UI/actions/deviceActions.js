import 'whatwg-fetch';
import jsonld from 'jsonld';
import parseLinkHeader from 'parse-link-header';
import {Hydra} from 'heracles';
 
export const DEVICES_COLLECTION_GET = 'DEVICES_COLLECTION_GET';

export const getDeviceCollection = () => {
    return async dispatch => {
        
        const res = await Hydra.loadResource('http://localhost:5000/api/nodes');

        return dispatch({
            type: DEVICES_COLLECTION_GET,
            deviceCollection: res
        });
    }
};
