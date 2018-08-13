#!/usr/bin/env sage

# use installed sageMath to compute the matrices in a modular ring
from sage.all import *

def elementwise(operator, M, N):
    assert(M.parent() == N.parent())
    nc, nr = M.ncols(), M.nrows()
    A = zero_matrix(2,2)
    for r in xrange(nr):
        for c in xrange(nc):
            A[r,c] = operator(M[r,c], N[r,c])
    return A


def main():
    # reading and creating of matrices
    with open('pubkey.txt', 'r') as f:
        n = f.readline().replace('\n', '').replace('n=', '')
        n = int(n)
        alpha = f.readline().replace('\n', '').replace('\\alpha=', '').replace(' ', ',')
        beta = f.readline().replace('\n', '').replace('\\beta=', '').replace(' ', ',')
        gamma = f.readline().replace('\n', '').replace('\\gamma=', '').replace(' ', ',')
        alpha = eval(alpha)
        beta = eval(beta)
        gamma = eval(gamma)

    # reading and creating of matrices
    with open('sarahs_enc.enc','r') as enc:
        mu = enc.readline().replace('\\mu^\\prime=', '').replace('\n', '').replace(' ', ',')
        epsilon = enc.readline().replace('\\epsilon=', '').replace('\n', '').replace(' ', ',')
        mu = eval(mu)
        epsilon = eval(epsilon)

    #h * (beta - aplha ^ (-1)) = alpha ^ (-1) * gamma - gamma * beta

    ring = IntegerModRing(n)
    alpha = Matrix(ring, alpha)
    beta = Matrix(ring, beta)
    gamma = Matrix(ring, gamma)
    invalpha = alpha.inverse()
    epsilon = Matrix(ring, epsilon)
    mu = Matrix(ring, mu)

    # compute the decryption matrix H

    h = elementwise(operator.div, (invalpha * gamma) - (gamma * beta), beta - invalpha)
    H = (h[0][0] * identity_matrix(2)) + gamma


    # decryption

    la = H.inverse() * epsilon * H
    plaintext = la * mu * la

    print 'plain 1,1: ', str(hex(int(plaintext[0][0]))).replace('0x', '')[:-1].decode('hex')
    print 'plain 1,2: ', str(hex(int(plaintext[0][1]))).replace('0x', '')[:-1].decode('hex')
    print 'plain 2,1: ', str(hex(int(plaintext[1][0]))).replace('0x', '')[:-1].decode('hex')
    print 'plain 2,2: ', str(hex(int(plaintext[1][1]))).replace('0x', '')[:-1].decode('hex')


if __name__ == '__main__':
    main()
