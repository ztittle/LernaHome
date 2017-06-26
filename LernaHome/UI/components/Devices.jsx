import React from 'react';
import { connect } from 'react-redux'
import {deviceActions} from '../actions'

const Devices = ({deviceCollection}) => 
    <div>
        <h1>Devices</h1>
        {deviceCollection['http://www.w3.org/ns/hydra/core#member'].map(device => 
            <h2>{device['http://example.com/zwave/hasSpecificType']['http://www.w3.org/2000/01/rdf-schema#label']}</h2>
        )}
    </div>;

class DevicesContainer extends React.Component {
    componentDidMount() {
        this.props.dispatch(deviceActions.getDeviceCollection());
    }

    render() {
        return <Devices {...this.props}/>
    }
}

const mapStateToProps = (state) => ({
    deviceCollection: state.deviceCollection
})

export default connect(mapStateToProps)(DevicesContainer);