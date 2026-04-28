import { render, screen } from '@testing-library/react';
import { AuthProvider, useAuth } from './AuthContext';


jest.mock('../services/authService');
jest.mock('../utils/storage');

const TestComponent = () => {
  const { isAuthenticated, login, logout } = useAuth();
  return (
    <div>
      <div data-testid="auth-status">
        {isAuthenticated ? 'Authenticated' : 'Not Authenticated'}
      </div>
      <button onClick={() => login('test@example.com', 'password')}>Login</button>
      <button onClick={logout}>Logout</button>
    </div>
  );
};

describe('AuthContext', () => {
  test('starts not authenticated', () => {
    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    );

    expect(screen.getByTestId('auth-status')).toHaveTextContent('Not Authenticated');
  });

  test('login makes user authenticated', async () => {
    const { authService } = require('../services/authService');
    authService.login.mockResolvedValue({
      token: 'jwt-token',
      user: { id: 1, email: 'test@example.com' }
    });

    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    );

    const loginButton = screen.getByText('Login');
    loginButton.click();

  });
});
