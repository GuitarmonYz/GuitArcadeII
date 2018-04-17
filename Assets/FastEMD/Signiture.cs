namespace FastEMD {
    public class Signiture {
        private int numberOfFeatures;
        private Feature[] features;
        private float[] weights;
        public int getNumberOfFeatures(){
            return this.numberOfFeatures;
        }
        public Feature[] getFeatures(){
            return this.features;
        }
        public void setFeatures(Feature[] features){
            this.features = features;
            this.numberOfFeatures = features.Length;
        }
        public float[] getWeights(){
            return this.weights;
        }
        public void setWeights(float[] weights) {
            this.weights = weights;
        }

    }
}