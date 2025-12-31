#!/bin/bash

echo "🚀 Deploying MyShop to Kubernetes..."

# Create namespace
kubectl apply -f namespace.yaml

# Apply secrets and configmap
kubectl apply -f secrets.yaml
kubectl apply -f configmap.yaml

# Deploy databases
echo "📦 Deploying databases..."
kubectl apply -f databases/

# Wait for databases to be ready
echo "⏳ Waiting for databases..."
kubectl wait --for=condition=ready pod -l app=mongodb -n myshop --timeout=120s
kubectl wait --for=condition=ready pod -l app=redis -n myshop --timeout=120s
kubectl wait --for=condition=ready pod -l app=mssql -n myshop --timeout=180s

# Deploy services
echo "🔧 Deploying microservices..."
kubectl apply -f services/

# Wait for services
echo "⏳ Waiting for services..."
kubectl wait --for=condition=ready pod -l app=identityserver -n myshop --timeout=180s
kubectl wait --for=condition=ready pod -l app=catalog -n myshop --timeout=120s
kubectl wait --for=condition=ready pod -l app=basket -n myshop --timeout=120s
kubectl wait --for=condition=ready pod -l app=order -n myshop --timeout=120s
kubectl wait --for=condition=ready pod -l app=discount -n myshop --timeout=120s
kubectl wait --for=condition=ready pod -l app=frontend -n myshop --timeout=120s

# Apply ingress
echo "🌐 Applying ingress..."
kubectl apply -f ingress.yaml

echo "✅ Deployment complete!"
echo ""
echo "📊 Status:"
kubectl get pods -n myshop
echo ""
kubectl get services -n myshop